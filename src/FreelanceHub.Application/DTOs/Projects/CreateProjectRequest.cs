namespace FreelanceHub.Application.DTOs.Projects;

public class CreateProjectRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public string Type { get; set; } = "Fixed";
    public decimal Budget { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime? Deadline { get; set; }
    public int EstimatedDuration { get; set; }
    public string DurationUnit { get; set; } = "Days";
    public string RequiredExperienceLevel { get; set; } = "Intermediate";
}
