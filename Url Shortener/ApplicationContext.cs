using Microsoft.EntityFrameworkCore;
using Url_Shortener.Entities;
using Url_Shortener.Services;

namespace Url_Shortener
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options):base (options) { }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortenedUrl>(builder =>
            {
                builder.Property(s => s.Code).HasMaxLength(UrlShorteningServices.NumberOfCharsInShortLink);

                builder.HasIndex(s => s.Code).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
