using DonStarveWikiTranslator.Models;
using DonStarveWikiTranslator.Modules;
using System.Data.Entity;

namespace DonStarveWikiTranslator.Data
{
    /// <summary>
    /// Entity Framework DbContext for wiki database
    /// </summary>
    public class WikiContext : DbContext
    {
        public WikiContext() : base(AppConfig.ConnectionString)
        {
            // Initialize database if it doesn't exist
            Database.SetInitializer(new CreateDatabaseIfNotExists<WikiContext>());
        }

        public DbSet<WikiArticle> WikiArticles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure WikiArticle entity
            modelBuilder.Entity<WikiArticle>()
                .ToTable("WikiArticles");

            modelBuilder.Entity<WikiArticle>()
                .Property(w => w.Title)
                .IsRequired()
                .HasMaxLength(500);

            modelBuilder.Entity<WikiArticle>()
                .Property(w => w.VietnameseTitle)
                .IsOptional()
                .HasMaxLength(500);

            modelBuilder.Entity<WikiArticle>()
                .Property(w => w.EnglishUrl)
                .IsOptional()
                .HasMaxLength(1000);

            modelBuilder.Entity<WikiArticle>()
                .Property(w => w.VietnameseUrl)
                .IsOptional()
                .HasMaxLength(1000);

            modelBuilder.Entity<WikiArticle>()
                .HasIndex(w => w.Title)
                .IsUnique();

            modelBuilder.Entity<WikiArticle>()
                .Property(w => w.EnglishContent)
                .IsOptional();

            modelBuilder.Entity<WikiArticle>()
                .Property(w => w.VietnameseContent)
                .IsOptional();

            modelBuilder.Entity<WikiArticle>()
                .Property(w => w.Status)
                .IsRequired();

            modelBuilder.Entity<WikiArticle>()
                .Property(w => w.LastSyncDate)
                .IsRequired();
        }
    }
}
