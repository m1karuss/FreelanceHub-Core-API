using FluentAssertions;
using FreelanceHub.Application.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FreelanceHub.UnitTests.Services;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"JwtSettings:SecretKey", "YourSuperSecretKeyThatIsAtLeast32CharactersLong!"},
            {"JwtSettings:Issuer", "FreelanceHub"},
            {"JwtSettings:Audience", "FreelanceHubUsers"},
            {"JwtSettings:AccessTokenExpirationMinutes", "15"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _tokenService = new TokenService(configuration);
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnValidToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var role = "Freelancer";

        // Act
        var token = _tokenService.GenerateAccessToken(userId, email, role);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT has 3 parts
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnUniqueTokens()
    {
        // Act
        var token1 = _tokenService.GenerateRefreshToken();
        var token2 = _tokenService.GenerateRefreshToken();

        // Assert
        token1.Should().NotBeNullOrEmpty();
        token2.Should().NotBeNullOrEmpty();
        token1.Should().NotBe(token2);
    }

    [Fact]
    public async Task GeneratePasswordResetTokenAsync_ShouldReturnToken()
    {
        // Act
        var token = await _tokenService.GeneratePasswordResetTokenAsync();

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Length.Should().BeGreaterThan(20);
    }
}
