using FluentAssertions;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Enums;
using Xunit;

namespace FreelanceHub.UnitTests.Entities;

public class UserTests
{
    [Fact]
    public void User_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";

        // Act
        var user = new User
        {
            Id = userId,
            Email = email,
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Freelancer
        };

        // Assert
        user.Id.Should().Be(userId);
        user.Email.Should().Be(email);
        user.FirstName.Should().Be("John");
        user.LastName.Should().Be("Doe");
        user.Role.Should().Be(UserRole.Freelancer);
    }
}
