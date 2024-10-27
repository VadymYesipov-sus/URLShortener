using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using URLShortener.Data;
using URLShortener.Models;
using URLShortener.Models.UserModels;
using URLShortener.Services;

namespace URLShortener.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrlController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UrlShorteningService _urlShorteningService;
        private readonly UserManager<User> _userManager;

        public UrlController(ApplicationDbContext context, UrlShorteningService urlShorteningService, UserManager<User> userManager)
        {
            _context = context;
            _urlShorteningService = urlShorteningService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost("shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request)
        {

            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            {
                return BadRequest("Your URL is wrong!");
            }

            //decided to use UserManager service instead because of posibility to access more user details
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //var user = await _userManager.GetUserAsync(User);

            if (userId == null)
            {
                return Unauthorized("User is not authenticated. Log in and try again!");
            }

            var existingUrl = await _context.ShortenedUrls
                .FirstOrDefaultAsync(url => url.LongUrl == request.Url);

            if (existingUrl != null)
            {
                return Conflict("This URL has already been shortened.");
            }

            var code = await _urlShorteningService.GenerateUniqueCode();

            var shortenedUrl = new ShortenedUrl
            {
                Id = Guid.NewGuid(),
                LongUrl = request.Url,
                Code = code,
                ShortUrl = $"{Request.Scheme}://{Request.Host}/api/url/{code}",
                CreatedOnUtc = DateTime.Now,
                UserId = userId,
            };


            _context.ShortenedUrls.Add(shortenedUrl);

            await _context.SaveChangesAsync();

            return Ok(shortenedUrl.ShortUrl);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectUrl(string code)
        {
            var shortenedUrl = await _context.ShortenedUrls
                .FirstOrDefaultAsync(s => s.Code == code);

            if (shortenedUrl == null)
            {
                return NotFound();
            }

            return Redirect(shortenedUrl.LongUrl);
        }

        [HttpGet("info/{code}")]
        public async Task<ActionResult<ShortenedUrl>> GetUrlByCode(string code)
        {
            var shortenedUrl = await _context.ShortenedUrls
                .FirstOrDefaultAsync(url => url.Code == code);

            if (shortenedUrl == null)
            {
                return NotFound(new { message = "URL not found." });
            }

            // Return the found URL
            return Ok(shortenedUrl);
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteUrl(string code)
        {
            var url = await _context.ShortenedUrls
                .FirstOrDefaultAsync(u => u.Code == code);

            if (url == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isAdmin = User.IsInRole("admin");

            if (url.UserId != userId && !isAdmin)
            {
                return Forbid(); 
            }

            _context.ShortenedUrls.Remove(url);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("all")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAllUrls()
        {
            if (!User.IsInRole("admin"))
            {
                return Forbid();
            }

            var allUrls = await _context.ShortenedUrls.ToListAsync();
            _context.ShortenedUrls.RemoveRange(allUrls);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ShortenedUrl>>> GetUrls()
        {
            var urls = await _context.ShortenedUrls.ToListAsync();
        //we don't want to include primary and forgein keys like Id's in server response
            return Ok(urls.Select(url => new
            {
                url.LongUrl,
                url.ShortUrl,
                url.Code,
                url.CreatedOnUtc,
                url.UserId,
            }));
        }

    }
}
