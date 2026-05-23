using FreelanceHub.Application.DTOs.Common;
using FreelanceHub.Application.DTOs.Projects;

namespace FreelanceHub.Application.Services.Interfaces;

public interface IProjectService
{
    Task<ProjectDto> CreateProjectAsync(Guid clientId, CreateProjectRequest request);
    Task<ProjectDto> GetProjectByIdAsync(Guid projectId);
    Task<PagedResponse<ProjectDto>> GetProjectsAsync(string? searchTerm, string? category, decimal? minBudget, decimal? maxBudget, int pageNumber, int pageSize);
    Task<IEnumerable<ProjectDto>> GetMyProjectsAsync(Guid clientId);
    Task<ProjectDto> UpdateProjectAsync(Guid projectId, Guid clientId, UpdateProjectRequest request);
    Task<bool> DeleteProjectAsync(Guid projectId, Guid clientId);
    Task<bool> PublishProjectAsync(Guid projectId, Guid clientId);
    Task<bool> CloseProjectAsync(Guid projectId, Guid clientId);
    Task<bool> AwardProjectAsync(Guid projectId, Guid clientId, Guid freelancerId);
}
