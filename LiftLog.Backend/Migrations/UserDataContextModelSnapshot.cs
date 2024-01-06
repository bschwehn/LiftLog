﻿// <auto-generated />
using System;
using LiftLog.Backend.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LiftLog.Backend.Migrations
{
    [DbContext(typeof(UserDataContext))]
    partial class UserDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LiftLog.Backend.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<byte[]>("EncryptedCurrentPlan")
                        .HasColumnType("bytea");

                    b.Property<byte[]>("EncryptedName")
                        .HasColumnType("bytea");

                    b.Property<byte[]>("EncryptedProfilePicture")
                        .HasColumnType("bytea");

                    b.Property<byte[]>("EncryptionIV")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("LastAccessed")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("LiftLog.Backend.Models.UserEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<byte[]>("EncryptedEvent")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<byte[]>("EncryptionIV")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<DateTimeOffset>("Expiry")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("LastAccessed")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserEvents");
                });

            modelBuilder.Entity("LiftLog.Backend.Models.UserFollowSecret", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserFollowSecrets");
                });

            modelBuilder.Entity("LiftLog.Backend.Models.UserInboxItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<byte[][]>("EncryptedMessage")
                        .IsRequired()
                        .HasColumnType("bytea[]");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserInboxItems");
                });

            modelBuilder.Entity("LiftLog.Backend.Models.UserEvent", b =>
                {
                    b.HasOne("LiftLog.Backend.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("LiftLog.Backend.Models.UserFollowSecret", b =>
                {
                    b.HasOne("LiftLog.Backend.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("LiftLog.Backend.Models.UserInboxItem", b =>
                {
                    b.HasOne("LiftLog.Backend.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
