using Microsoft.EntityFrameworkCore;
using URLShortener.Data;

namespace URLShortener.Extensions
{
    //this extension method automatically applies all migrations to the database,
    //making sure that it is up-to-date, so we don't have to type dotnet ef database update
    //every time we make changes to the database
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Database.Migrate();
        }
    }
}
