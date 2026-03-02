using Masofa.Common.Models.Uav;
using Microsoft.EntityFrameworkCore;

namespace Masofa.DataAccess
{
    public partial class MasofaUAVDbContext : DbContext
    {
        public MasofaUAVDbContext(DbContextOptions<MasofaUAVDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            this.ApplyLocalizationStringSettings(modelBuilder);
            modelBuilder.Entity<UAVFlyPath>().ToTable("UAVFlyPath");
            modelBuilder.Entity<UAVPhotoCollection>().ToTable("UAVPhotoCollection");
            modelBuilder.Entity<UAVPhoto>().ToTable("UAVPhoto");
            modelBuilder.Entity<UAVPhotoCollectionRelation>().ToTable("UAVPhotoCollectionRelation");
            modelBuilder.Entity<UAVPhotoCollection>()
                .HasOne<UAVFlyPath>()
                .WithMany()
                .HasForeignKey(c => c.UAVFlyPathId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UAVPhoto>()
                .HasOne<UAVPhotoCollection>()
                .WithMany()
                .HasForeignKey(p => p.UAVPhotoCollectionId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UAVPhotoCollectionRelation>()
                .HasOne<UAVPhotoCollection>()
                .WithMany()
                .HasForeignKey(r => r.UAVPhotoCollectionId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<UAVFlyPath> UAVFlyPaths { get; set; }
        public DbSet<UAVPhotoCollection> UAVPhotoCollections { get; set; }
        public DbSet<UAVPhoto> UAVPhotos { get; set; }
        public DbSet<UAVPhotoCollectionRelation> UAVPhotoCollectionRelations { get; set; }
    }
}