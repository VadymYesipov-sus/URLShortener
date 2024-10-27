using Microsoft.EntityFrameworkCore;
using URLShortener.Data;

namespace URLShortener.Services
{
    public class UrlShorteningService
    {
        public const int NumberOfCharsInShortLink = 7;
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        //creating random to take 7 random characters from Alphabet string
        //this combination of 7 chars will represent our shortened url.
        private readonly Random _random = new();
        private readonly ApplicationDbContext _dbContext;

        public UrlShorteningService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GenerateUniqueCode()
        {
            var codeChars = new char[NumberOfCharsInShortLink];

            //wrapping this piece of code in while loop to generate non-duplicate code.
            
            //the only problem is that with increasing amounts of unique codes in database,
            //we will have to perform several queries to the database. to avoid it, we can create
            //unique codes ahead of time.

            while (true)
            {
                for (int i = 0; i < codeChars.Length; i++)
                {
                    int randomIndex = _random.Next(Alphabet.Length - 1);

                    codeChars[i] = Alphabet[randomIndex];
                }

                var code = new string(codeChars);

            //when we get unique code, we exit the loop

                if (!await _dbContext.ShortenedUrls.AnyAsync(s => s.Code == code))
                {
                    return code;
                }
            }
        }
    }
}
