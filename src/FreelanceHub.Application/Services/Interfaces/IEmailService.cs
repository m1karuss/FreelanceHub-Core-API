namespace FreelanceHub.Application.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendPasswordResetEmailAsync(string to, string resetToken);
    Task SendEmailVerificationAsync(string to, string verificationToken);
    Task SendWelcomeEmailAsync(string to, string firstName);
}
