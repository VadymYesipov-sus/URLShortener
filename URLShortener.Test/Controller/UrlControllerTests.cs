using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using URLShortener.Controllers;
using URLShortener.Data;
using URLShortener.Models.UserModels;
using URLShortener.Models;
using URLShortener.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class UrlControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly UrlShorteningService _urlShorteningService;
    private readonly UserManager<User> _userManager;
    private readonly UrlController _controller;

    public UrlControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(options);

        _urlShorteningService = new UrlShorteningService(_context);

        _controller = new UrlController(_context, _urlShorteningService, _userManager);

    }



    [Fact]
    public async Task UrlController_ShortenUrl_ReturnsOk()
    {
        // Arrange
        var request = new ShortenUrlRequest { Url = "http://vadymvadymvadym.com" };
        var userId = "test-user-id";
        var generatedCode = "abc123";

        var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
        new Claim(ClaimTypes.NameIdentifier, userId)
        }));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = userClaims }
        };
        var code = _urlShorteningService.GenerateUniqueCode();


        // Act
        var result = await _controller.ShortenUrl(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var shortUrl = Assert.IsType<string>(okResult.Value);

        var expectedUrl = $"http://localhost:7203/api/url/{generatedCode}";
        Assert.Contains(generatedCode, expectedUrl);
    }
}