﻿// <auto-generated />
using System;
using GameStore.DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GameStore.DAL.Migrations
{
    [DbContext(typeof(GameStoreContext))]
    [Migration("20220514155350_AddedCommentTypeToCommentEntity")]
    partial class AddedCommentTypeToCommentEntity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.16")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GameStore.DAL.Entities.CommentEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateOfDelete")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Type")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Standalone");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("ParentId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.GameEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DateOfDelete")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Discontinued")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid?>("PublisherId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<short>("UnitsInStock")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique()
                        .HasFilter("[Key] IS NOT NULL");

                    b.HasIndex("PublisherId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.GameGenreEntity", b =>
                {
                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GenreId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("GameId", "GenreId");

                    b.HasIndex("GenreId");

                    b.ToTable("GamesGenres");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.GamePlatformTypeEntity", b =>
                {
                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PlatformId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("GameId", "PlatformId");

                    b.HasIndex("PlatformId");

                    b.ToTable("GamesPlatformTypes");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.GenreEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DateOfDelete")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.HasIndex("ParentId");

                    b.ToTable("Genres");

                    b.HasData(
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            Name = "Strategy"
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000002"),
                            IsDeleted = false,
                            Name = "RTS",
                            ParentId = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000003"),
                            IsDeleted = false,
                            Name = "TBS",
                            ParentId = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000004"),
                            IsDeleted = false,
                            Name = "RPG"
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000005"),
                            IsDeleted = false,
                            Name = "Sports"
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000006"),
                            IsDeleted = false,
                            Name = "Races"
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000007"),
                            IsDeleted = false,
                            Name = "Rally",
                            ParentId = new Guid("00000000-0000-0000-0000-000000000006")
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000008"),
                            IsDeleted = false,
                            Name = "Arcade",
                            ParentId = new Guid("00000000-0000-0000-0000-000000000006")
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000009"),
                            IsDeleted = false,
                            Name = "Formula",
                            ParentId = new Guid("00000000-0000-0000-0000-000000000006")
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000010"),
                            IsDeleted = false,
                            Name = "Off-road",
                            ParentId = new Guid("00000000-0000-0000-0000-000000000006")
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000011"),
                            IsDeleted = false,
                            Name = "Action"
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000012"),
                            IsDeleted = false,
                            Name = "FPS",
                            ParentId = new Guid("00000000-0000-0000-0000-000000000011")
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000013"),
                            IsDeleted = false,
                            Name = "TPS",
                            ParentId = new Guid("00000000-0000-0000-0000-000000000011")
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000014"),
                            IsDeleted = false,
                            Name = "Misc"
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000015"),
                            IsDeleted = false,
                            Name = "Adventure"
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000016"),
                            IsDeleted = false,
                            Name = "Puzzle & Skill"
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0000-000000000017"),
                            IsDeleted = false,
                            Name = "Others"
                        });
                });

            modelBuilder.Entity("GameStore.DAL.Entities.LegacyKeyEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LegacyKey")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("LegacyKey")
                        .IsUnique()
                        .HasFilter("[LegacyKey] IS NOT NULL");

                    b.ToTable("LegacyKeys");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.OrderDetailsEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DateOfDelete")
                        .HasColumnType("datetime2");

                    b.Property<double>("Discount")
                        .HasColumnType("float");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<short>("Quantity")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.OrderEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DateOfDelete")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ExpirationDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Open");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.PlatformTypeEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DateOfDelete")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Type")
                        .IsUnique()
                        .HasFilter("[Type] IS NOT NULL");

                    b.ToTable("PlatformTypes");

                    b.HasData(
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0001-000000000001"),
                            IsDeleted = false,
                            Type = "mobile"
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0001-000000000002"),
                            IsDeleted = false,
                            Type = "browser"
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0001-000000000003"),
                            IsDeleted = false,
                            Type = "desktop"
                        },
                        new
                        {
                            Id = new Guid("00000000-0000-0000-0001-000000000004"),
                            IsDeleted = false,
                            Type = "console"
                        });
                });

            modelBuilder.Entity("GameStore.DAL.Entities.PublisherEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CompanyName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("DateOfDelete")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomePage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("CompanyName")
                        .IsUnique()
                        .HasFilter("[CompanyName] IS NOT NULL");

                    b.ToTable("Publishers");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.CommentEntity", b =>
                {
                    b.HasOne("GameStore.DAL.Entities.GameEntity", "Game")
                        .WithMany("Comments")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameStore.DAL.Entities.CommentEntity", "Parent")
                        .WithMany("Replies")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Game");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.GameEntity", b =>
                {
                    b.HasOne("GameStore.DAL.Entities.PublisherEntity", "Publisher")
                        .WithMany("PublishedGames")
                        .HasForeignKey("PublisherId");

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.GameGenreEntity", b =>
                {
                    b.HasOne("GameStore.DAL.Entities.GameEntity", "Game")
                        .WithMany("GameGenres")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameStore.DAL.Entities.GenreEntity", "Genre")
                        .WithMany("GameGenres")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Genre");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.GamePlatformTypeEntity", b =>
                {
                    b.HasOne("GameStore.DAL.Entities.GameEntity", "Game")
                        .WithMany("GamesPlatformTypes")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameStore.DAL.Entities.PlatformTypeEntity", "PlatformType")
                        .WithMany("GamesPlatformTypes")
                        .HasForeignKey("PlatformId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("PlatformType");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.GenreEntity", b =>
                {
                    b.HasOne("GameStore.DAL.Entities.GenreEntity", "Parent")
                        .WithMany("SubGenres")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.LegacyKeyEntity", b =>
                {
                    b.HasOne("GameStore.DAL.Entities.GameEntity", "Game")
                        .WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.OrderDetailsEntity", b =>
                {
                    b.HasOne("GameStore.DAL.Entities.OrderEntity", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GameStore.DAL.Entities.GameEntity", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.CommentEntity", b =>
                {
                    b.Navigation("Replies");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.GameEntity", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("GameGenres");

                    b.Navigation("GamesPlatformTypes");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.GenreEntity", b =>
                {
                    b.Navigation("GameGenres");

                    b.Navigation("SubGenres");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.OrderEntity", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.PlatformTypeEntity", b =>
                {
                    b.Navigation("GamesPlatformTypes");
                });

            modelBuilder.Entity("GameStore.DAL.Entities.PublisherEntity", b =>
                {
                    b.Navigation("PublishedGames");
                });
#pragma warning restore 612, 618
        }
    }
}
