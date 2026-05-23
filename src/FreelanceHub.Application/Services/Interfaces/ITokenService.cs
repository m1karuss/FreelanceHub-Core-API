namespace FreelanceHub.Application.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, string role);
    string GenerateRefreshToken();
    Task<string> GenerateEmailVerificationTokenAsync();
    Task<string> GeneratePasswordResetTokenAsync();
}
