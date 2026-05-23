namespace FreelanceHub.Application.DTOs.Projects;

public class ProjectDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime? Deadline { get; set; }
    public int EstimatedDuration { get; set; }
    public string DurationUnit { get; set; } = string.Empty;
    public string RequiredExperienceLevel { get; set; } = string.Empty;
    public int ViewsCount { get; set; }
    public int BidsCount { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
