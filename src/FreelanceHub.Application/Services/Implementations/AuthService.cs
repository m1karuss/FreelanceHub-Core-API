using FreelanceHub.Application.DTOs.Auth;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Interfaces;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace FreelanceHub.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        UserManager<User> userManager,
        IConfiguration configuration,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _userManager = userManager;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new ConflictException("User with this email already exists");
        }

        // Create new user
        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var userRole))
        {
            throw new ValidationException(new[] { $"Invalid role: '{request.Role}'. Valid values are: {string.Join(", ", Enum.GetNames<UserRole>())}" });
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Role = userRole,
            Status = UserStatus.Active,
            VerificationStatus = VerificationStatus.Unverified,
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow,
            ProfileCompletionPercentage = 40
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            throw new ValidationException(errors);
        }

        // Generate email verification token
        var verificationToken = await _tokenService.GenerateEmailVerificationTokenAsync();
        user.EmailVerificationToken = verificationToken;
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);

        await _unitOfWork.Users.UpdateAsync(user);

        // Create profile based on role and persist it
        if (user.Role == UserRole.Freelancer)
        {
            var freelancerProfile = new FreelancerProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.FreelancerProfiles.AddAsync(freelancerProfile);
        }
        else if (user.Role == UserRole.Client)
        {
            var clientProfile = new ClientProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.ClientProfiles.AddAsync(clientProfile);
        }

        await _unitOfWork.SaveChangesAsync();

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                EmailVerified = user.EmailVerified
            }
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

        if (user == null)
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        // Check if account is locked
        if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow)
        {
            throw new ForbiddenException($"Account is locked until {user.LockedUntil.Value:yyyy-MM-dd HH:mm:ss} UTC");
        }

        // Verify password
        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValid)
        {
            // Increment failed login attempts
            user.FailedLoginAttempts++;

            var maxAttempts = _configuration.GetValue<int>("ApplicationSettings:MaxLoginAttempts");
            if (user.FailedLoginAttempts >= maxAttempts)
            {
                var lockoutMinutes = _configuration.GetValue<int>("ApplicationSettings:AccountLockoutMinutes");
                user.LockedUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes);
            }

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            throw new UnauthorizedException("Invalid email or password");
        }

        // Reset failed attempts on successful login
        user.FailedLoginAttempts = 0;
        user.LockedUntil = null;
        user.LastLoginAt = DateTime.UtcNow;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                EmailVerified = user.EmailVerified
            }
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var token = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);

        if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid or expired refresh token");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(token.UserId);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        // Revoke old token
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        await _unitOfWork.RefreshTokens.UpdateAsync(token);

        // Generate new tokens
        var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString());
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Save new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _unitOfWork.RefreshTokens.AddAsync(newRefreshTokenEntity);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                EmailVerified = user.EmailVerified
            }
        };
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var token = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);

        if (token == null)
        {
            return false;
        }

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        await _unitOfWork.RefreshTokens.UpdateAsync(token);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> VerifyEmailAsync(string token)
    {
        var user = await _unitOfWork.Users.GetByEmailVerificationTokenAsync(token);

        if (user == null || user.EmailVerificationTokenExpiry < DateTime.UtcNow)
        {
            throw new BadRequestException("Invalid or expired verification token");
        }

        user.EmailVerified = true;
        user.EmailVerifiedAt = DateTime.UtcNow;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;
        user.VerificationStatus = VerificationStatus.Verified;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email);

        if (user == null)
        {
            // Don't reveal that user doesn't exist
            return true;
        }

        var resetToken = await _tokenService.GeneratePasswordResetTokenAsync();
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Send password reset email
        await _emailService.SendPasswordResetEmailAsync(user.Email!, resetToken);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByPasswordResetTokenAsync(token);

        if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            throw new BadRequestException("Invalid or expired reset token");
        }

        var resetResult = await _userManager.RemovePasswordAsync(user);
        if (!resetResult.Succeeded)
        {
            throw new BadRequestException("Failed to reset password");
        }

        var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
        if (!addPasswordResult.Succeeded)
        {
            var errors = addPasswordResult.Errors.Select(e => e.Description);
            throw new ValidationException(errors);
        }

        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
