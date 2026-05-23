using FreelanceHub.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace FreelanceHub.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderName;
    private readonly string _username;
    private readonly string _password;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var emailSettings = _configuration.GetSection("EmailSettings");
        _smtpServer = emailSettings["SmtpServer"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
        _senderEmail = emailSettings["SenderEmail"] ?? "noreply@freelancehub.com";
        _senderName = emailSettings["SenderName"] ?? "FreelanceHub";
        _username = emailSettings["Username"] ?? "";
        _password = emailSettings["Password"] ?? "";
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_username, _password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_senderEmail, _senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);

            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            throw;
        }
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetToken)
    {
        var subject = "Password Reset Request - FreelanceHub";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333;'>Password Reset Request</h2>
                    <p>You have requested to reset your password for your FreelanceHub account.</p>
                    <p>Please use the following token to reset your password:</p>
                    <div style='background-color: #f5f5f5; padding: 15px; margin: 20px 0; border-radius: 5px;'>
                        <code style='font-size: 16px; color: #d63384;'>{resetToken}</code>
                    </div>
                    <p>This token will expire in 1 hour.</p>
                    <p>If you did not request a password reset, please ignore this email.</p>
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                    <p style='color: #666; font-size: 12px;'>
                        This is an automated email from FreelanceHub. Please do not reply to this email.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendEmailVerificationAsync(string to, string verificationToken)
    {
        var subject = "Verify Your Email - FreelanceHub";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333;'>Welcome to FreelanceHub!</h2>
                    <p>Thank you for registering with FreelanceHub.</p>
                    <p>Please verify your email address by using the following verification token:</p>
                    <div style='background-color: #f5f5f5; padding: 15px; margin: 20px 0; border-radius: 5px;'>
                        <code style='font-size: 16px; color: #0d6efd;'>{verificationToken}</code>
                    </div>
                    <p>This token will expire in 24 hours.</p>
                    <p>If you did not create an account, please ignore this email.</p>
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                    <p style='color: #666; font-size: 12px;'>
                        This is an automated email from FreelanceHub. Please do not reply to this email.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string to, string firstName)
    {
        var subject = "Welcome to FreelanceHub!";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333;'>Welcome to FreelanceHub, {firstName}!</h2>
                    <p>Your email has been successfully verified.</p>
                    <p>You can now start using FreelanceHub to:</p>
                    <ul>
                        <li>Post projects and hire talented freelancers</li>
                        <li>Browse available projects and submit bids</li>
                        <li>Manage payments securely</li>
                        <li>Communicate with clients and freelancers</li>
                    </ul>
                    <p style='margin-top: 30px;'>
                        <a href='https://freelancehub.com'
                           style='background-color: #0d6efd; color: white; padding: 12px 30px;
                                  text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Get Started
                        </a>
                    </p>
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                    <p style='color: #666; font-size: 12px;'>
                        This is an automated email from FreelanceHub. Please do not reply to this email.
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(to, subject, body);
    }
}
