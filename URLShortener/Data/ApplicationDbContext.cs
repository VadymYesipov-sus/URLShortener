using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using URLShortener.Models;
using URLShortener.Models.UserModels;
using URLShortener.Services;

namespace URLShortener.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options)
            :base(options)
        {
            
        }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ShortenedUrl>(builder =>
            {
                //HasMaxLength helps with performance ny telling the database that shortened url is 7 characters at max as well as limiting how much data you can index
                builder.Property(s => s.Code).HasMaxLength(UrlShorteningService.NumberOfCharsInShortLink);
                //this constraint prevents duplicate codes from generating
                builder.HasIndex(s => s.Code).IsUnique();
            });

            modelBuilder.Entity<ShortenedUrl>()
                .HasOne(s => s.User)
                .WithMany(u => u.Urls)
                .HasForeignKey(s => s.UserId) 
                .OnDelete(DeleteBehavior.Cascade); // deletes all urls when user is deleted
        }

    }
}
