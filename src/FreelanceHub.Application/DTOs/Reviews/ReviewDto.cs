namespace FreelanceHub.Application.DTOs.Reviews;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public Guid RevieweeId { get; set; }
    public string RevieweeName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string? Pros { get; set; }
    public string? Cons { get; set; }
    public bool IsPublic { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
