using GameStore.DAL.Entities;
using GameStore.DAL.Entities.Localizations;
using GameStore.DomainModels.Enums;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DAL.Context
{
    public class GameStoreContext : DbContext
    {
        public DbSet<GameEntity> Games { get; set; }

        public DbSet<CommentEntity> Comments { get; set; }

        public DbSet<GenreEntity> Genres { get; set; }

        public DbSet<PlatformTypeEntity> PlatformTypes { get; set; }

        public DbSet<PublisherEntity> Publishers { get; set; }

        public DbSet<OrderDetailsEntity> OrderDetails { get; set; }

        public DbSet<OrderEntity> Orders { get; set; }

        public DbSet<LegacyKeyEntity> LegacyKeys { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<GenreCategoryMappingEntity> GenreCategory { get; set; }

        public DbSet<GoodsProductMappingEntity> GoodsProduct { get; set; }

        public DbSet<PublisherSupplierMappingEntity> PublisherSupplier { get; set; }

        public DbSet<LocalizationEntity> Localizations { get; set; }

        public DbSet<GameLocalizationEntity> GameLocalizations { get; set; }

        public DbSet<PlatformTypeLocalizaitionEntity> PlatformTypeLocalizations { get; set; }

        public DbSet<GenreLocalizationEntity> GenreLocalizations { get; set; }

        public DbSet<GameGenreEntity> GamesGenres { get; set; }

        public DbSet<GamePlatformTypeEntity> GamesPlatformTypes { get; set; }

        public GameStoreContext()
        {
        }

        public GameStoreContext(DbContextOptions<GameStoreContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Username)
                    .IsUnique();
                entity.HasIndex(x => x.Email)
                    .IsUnique();
                entity.Property(x => x.Role)
                    .HasDefaultValue(UserRoles.User)
                    .HasConversion<string>();

                entity.HasMany(x => x.Comments)
                    .WithOne(x => x.Author)
                    .HasForeignKey(x => x.AuthorId);
                entity.HasMany(x => x.Orders)
                    .WithOne(x => x.Customer)
                    .HasForeignKey(x => x.CustomerId);
            });

            modelBuilder.Entity<LegacyKeyEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.LegacyKey)
                    .IsUnique();
            });

            modelBuilder.Entity<PublisherEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.CompanyName)
                    .IsUnique();
                entity.HasMany(x => x.PublishedGames)
                    .WithOne(x => x.Publisher)
                    .HasForeignKey(x => x.PublisherId);
            });

            modelBuilder.Entity<OrderEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasMany(x => x.OrderDetails)
                    .WithOne(x => x.Order)
                    .HasForeignKey(x => x.OrderId);
                entity.Property(p => p.Status)
                    .HasDefaultValue(OrderStatus.Open)
                    .HasConversion<string>();
            });

            modelBuilder.Entity<OrderDetailsEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<GenreEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Name)
                    .IsUnique();
                entity.HasOne(x => x.Parent)
                    .WithMany(x => x.SubGenres)
                    .HasForeignKey(x => x.ParentId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CommentEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name);
                entity.Property(x => x.Body);
                entity.HasOne(x => x.Parent)
                    .WithMany(x => x.Replies)
                    .HasForeignKey(x => x.ParentId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(p => p.Type)
                    .HasDefaultValue(CommentType.Standalone)
                    .HasConversion<string>();
            });

            modelBuilder.Entity<GameEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.Key)
                    .IsUnique();
                entity.HasMany(x => x.Comments)
                    .WithOne(x => x.Game)
                    .HasForeignKey(x => x.GameId);
            });

            modelBuilder.Entity<GameGenreEntity>(entity =>
            {
                entity.HasKey(x => new { x.GameId, x.GenreId });

                entity.HasOne(x => x.Game)
                    .WithMany(x => x.GameGenres)
                    .HasForeignKey(x => x.GameId);

                entity.HasOne(x => x.Genre)
                    .WithMany(x => x.GameGenres)
                    .HasForeignKey(x => x.GenreId);
            });

            modelBuilder.Entity<GamePlatformTypeEntity>(entity =>
            {
                entity.HasKey(x => new { x.GameId, x.PlatformId });

                entity.HasOne(x => x.Game)
                    .WithMany(x => x.GamesPlatformTypes)
                    .HasForeignKey(x => x.GameId);

                entity.HasOne(x => x.PlatformType)
                    .WithMany(x => x.GamesPlatformTypes)
                    .HasForeignKey(x => x.PlatformId);
            });

            modelBuilder.Entity<PlatformTypeEntity>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.Type)
                    .IsUnique();
            });

            modelBuilder.Entity<PublisherSupplierMappingEntity>(entity =>
            {
                entity.HasKey(x => new { x.PublisherId, x.SupplierId });
            });

            modelBuilder.Entity<GenreCategoryMappingEntity>(entity =>
            {
                entity.HasKey(x => new { x.GenreId, x.CategoryId });
            });

            modelBuilder.Entity<GoodsProductMappingEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<GenreEntity>()
                .HasData(DataSeed.GetGenreSeed());

            modelBuilder.Entity<PlatformTypeEntity>()
                .HasData(DataSeed.GetPlatformTypeSeed());

            modelBuilder.Entity<UserEntity>()
                .HasData(DataSeed.GetUserSeed());

            modelBuilder.Entity<GenreCategoryMappingEntity>()
                .HasData(DataSeed.GetGenreCategoryMappingSeed());

            modelBuilder.Entity<LocalizationEntity>()
                .HasData(DataSeed.GetLocalizationsSeed());
        }
    }
}
