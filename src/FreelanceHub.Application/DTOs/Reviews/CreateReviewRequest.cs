namespace FreelanceHub.Application.DTOs.Reviews;

public class CreateReviewRequest
{
    public Guid ProjectId { get; set; }
    public Guid RevieweeId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string? Pros { get; set; }
    public string? Cons { get; set; }
    public bool IsPublic { get; set; } = true;
}
