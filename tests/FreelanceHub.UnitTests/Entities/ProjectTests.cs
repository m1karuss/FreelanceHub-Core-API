using FluentAssertions;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Enums;
using Xunit;

namespace FreelanceHub.UnitTests.Entities;

public class ProjectTests
{
    [Fact]
    public void Project_ShouldCalculateBudgetCorrectly()
    {
        // Arrange & Act
        var project = new Project
        {
            Budget = 5000,
            Currency = "USD"
        };

        // Assert
        project.Budget.Should().Be(5000);
        project.Currency.Should().Be("USD");
    }

    [Fact]
    public void Project_ShouldTransitionStatusCorrectly()
    {
        // Arrange
        var project = new Project
        {
            Status = ProjectStatus.Draft
        };

        // Act
        project.Status = ProjectStatus.Open;
        project.PublishedAt = DateTime.UtcNow;

        // Assert
        project.Status.Should().Be(ProjectStatus.Open);
        project.PublishedAt.Should().NotBeNull();
    }
}
