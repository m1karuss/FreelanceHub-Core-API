namespace FreelanceHub.Application.DTOs.Projects;

public class UpdateProjectRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Skills { get; set; }
    public decimal? Budget { get; set; }
    public string? Currency { get; set; }
    public DateTime? Deadline { get; set; }
    public int? EstimatedDuration { get; set; }
    public string? DurationUnit { get; set; }
    public string? RequiredExperienceLevel { get; set; }
}
