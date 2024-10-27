using Microsoft.AspNetCore.Identity;

namespace URLShortener.Models.UserModels
{
    public class User : IdentityUser
    {
        public ICollection<ShortenedUrl> Urls { get; set; } = new List<ShortenedUrl>(); // to avoid null reference exception

    }
}
