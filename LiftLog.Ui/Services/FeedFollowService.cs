using Google.Protobuf;
using LiftLog.Lib.Models;
using LiftLog.Lib.Services;
using LiftLog.Ui.Models;
using LiftLog.Ui.Store.Feed;
using Microsoft.Extensions.Logging;

namespace LiftLog.Ui.Services;

public class FeedFollowService(
    IFeedApiService feedApiService,
    IEncryptionService encryptionService,
    ILogger<FeedFollowService> logger
)
{
    public async Task<ApiResult> RequestToFollowAUserAsync(FeedIdentity identity, FeedUser toFollow)
    {
        var inboxMessage = new InboxMessageDao
        {
            FromUserId = identity.Id,
            FollowRequest = new FollowRequestDao { Name = identity.Name },
        };
        inboxMessage.Signature = ByteString.CopyFrom(
            await encryptionService.SignRsaPssSha256Async(
                FeedInboxDecryptionService.GetSignaturePayload(inboxMessage, toFollow.Id),
                identity.RsaKeyPair.PrivateKey
            )
        );
        var response = await feedApiService.PutInboxMessageAsync(
            new PutInboxMessageRequest(
                ToUserId: toFollow.Id,
                EncryptedMessage: (
                    await encryptionService.EncryptRsaOaepSha256Async(
                        inboxMessage.ToByteArray(),
                        toFollow.PublicKey
                    )
                ).DataChunks
            )
        );
        return response;
    }

    public async Task<ApiResult<string>> AcceptFollowRequestAsync(
        FeedIdentity identity,
        FollowRequest request,
        RsaPublicKey userPublicKey
    )
    {
        var followSecret = Guid.NewGuid().ToString();
        var putFollowSecretResponse = await feedApiService.PutUserFollowSecretAsync(
            new PutUserFollowSecretRequest(
                UserId: identity.Id,
                Password: identity.Password,
                FollowSecret: followSecret
            )
        );
        if (!putFollowSecretResponse.IsSuccess)
        {
            return ApiResult<string>.FromFailure(putFollowSecretResponse);
        }

        var inboxMessage = new InboxMessageDao
        {
            FromUserId = identity.Id,
            FollowResponse = new FollowResponseDao
            {
                Accepted = new FollowResponseAcceptedDao
                {
                    AesKey = ByteString.CopyFrom(identity.AesKey.Value),
                    FollowSecret = followSecret,
                },
            },
        };
        inboxMessage.Signature = ByteString.CopyFrom(
            await encryptionService.SignRsaPssSha256Async(
                FeedInboxDecryptionService.GetSignaturePayload(inboxMessage, request.UserId),
                identity.RsaKeyPair.PrivateKey
            )
        );
        var encryptedMessage = await encryptionService.EncryptRsaOaepSha256Async(
            inboxMessage.ToByteArray(),
            userPublicKey
        );
        var putResponse = await feedApiService.PutInboxMessageAsync(
            new PutInboxMessageRequest(
                ToUserId: request.UserId,
                EncryptedMessage: encryptedMessage.DataChunks
            )
        );

        if (!putResponse.IsSuccess)
        {
            return ApiResult<string>.FromFailure(putResponse);
        }

        return new ApiResult<string>(followSecret);
    }

    public async Task<ApiResult> DenyFollowRequestAsync(
        FeedIdentity identity,
        FollowRequest request,
        RsaPublicKey userPublicKey
    )
    {
        var inboxMessage = new InboxMessageDao
        {
            FromUserId = identity.Id,
            FollowResponse = new FollowResponseDao { Rejected = new FollowResponseRejectedDao() },
        };
        inboxMessage.Signature = ByteString.CopyFrom(
            await encryptionService.SignRsaPssSha256Async(
                FeedInboxDecryptionService.GetSignaturePayload(inboxMessage, request.UserId),
                identity.RsaKeyPair.PrivateKey
            )
        );
        RsaEncryptedData? encryptedMessage;
        try
        {
            encryptedMessage = await encryptionService.EncryptRsaOaepSha256Async(
                inboxMessage.ToByteArray(),
                userPublicKey
            );
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to encrypt inbox message");
            return new ApiResult(
                new ApiError(ApiErrorType.EncryptionError, "Failed to encrypt inbox message", e)
            );
        }

        var putResponse = await feedApiService.PutInboxMessageAsync(
            new PutInboxMessageRequest(
                ToUserId: request.UserId,
                EncryptedMessage: encryptedMessage.DataChunks
            )
        );
        return putResponse;
    }

    public async Task<ApiResult> RevokeFollowSecretAsync(FeedIdentity identity, string followSecret)
    {
        var putResponse = await feedApiService.DeleteUserFollowSecretAsync(
            new DeleteUserFollowSecretRequest(
                UserId: identity.Id,
                Password: identity.Password,
                FollowSecret: followSecret
            )
        );
        return putResponse;
    }
}
