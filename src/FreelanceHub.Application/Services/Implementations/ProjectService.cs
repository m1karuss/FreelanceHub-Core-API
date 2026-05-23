using FreelanceHub.Application.DTOs.Common;
using FreelanceHub.Application.DTOs.Projects;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Interfaces;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Interfaces;

namespace FreelanceHub.Application.Services.Implementations;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProjectDto> CreateProjectAsync(Guid clientId, CreateProjectRequest request)
    {
        var client = await _unitOfWork.Users.GetByIdAsync(clientId)
            ?? throw new NotFoundException("Client not found");

        if (client.Role != UserRole.Client)
            throw new ForbiddenException("Only clients can create projects");

        if (!Enum.TryParse<ProjectType>(request.Type, ignoreCase: true, out var projectType))
            throw new ValidationException(new[] { $"Invalid project type: '{request.Type}'" });

        if (!Enum.TryParse<ExperienceLevel>(request.RequiredExperienceLevel, ignoreCase: true, out var experienceLevel))
            throw new ValidationException(new[] { $"Invalid experience level: '{request.RequiredExperienceLevel}'" });

        var project = new Project
        {
            ClientId = clientId,
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Skills = request.Skills,
            Status = ProjectStatus.Draft,
            Type = projectType,
            Budget = request.Budget,
            Currency = request.Currency,
            Deadline = request.Deadline,
            EstimatedDuration = request.EstimatedDuration,
            DurationUnit = request.DurationUnit,
            RequiredExperienceLevel = experienceLevel,
            ViewsCount = 0,
            BidsCount = 0
        };

        await _unitOfWork.Projects.AddAsync(project);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(project, client);
    }

    public async Task<ProjectDto> GetProjectByIdAsync(Guid projectId)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new NotFoundException($"Project with ID {projectId} not found");

        var client = await _unitOfWork.Users.GetByIdAsync(project.ClientId);

        // Increment view count
        project.ViewsCount++;
        await _unitOfWork.Projects.UpdateAsync(project);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(project, client);
    }

    public async Task<PagedResponse<ProjectDto>> GetProjectsAsync(
        string? searchTerm, string? category, decimal? minBudget, decimal? maxBudget,
        int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var skip = (pageNumber - 1) * pageSize;
        var projects = await _unitOfWork.Projects.SearchProjectsAsync(
            searchTerm, category, minBudget, maxBudget, skip, pageSize);

        var totalCount = await _unitOfWork.Projects.GetProjectsCountAsync(
            searchTerm, category, minBudget, maxBudget);

        var dtos = new List<ProjectDto>();
        foreach (var project in projects)
        {
            var client = await _unitOfWork.Users.GetByIdAsync(project.ClientId);
            dtos.Add(MapToDto(project, client));
        }

        return new PagedResponse<ProjectDto>(dtos, totalCount, pageNumber, pageSize);
    }

    public async Task<IEnumerable<ProjectDto>> GetMyProjectsAsync(Guid clientId)
    {
        var client = await _unitOfWork.Users.GetByIdAsync(clientId)
            ?? throw new NotFoundException("User not found");

        var projects = await _unitOfWork.Projects.GetByClientIdAsync(clientId);

        return projects.Select(p => MapToDto(p, client));
    }

    public async Task<ProjectDto> UpdateProjectAsync(Guid projectId, Guid clientId, UpdateProjectRequest request)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new NotFoundException($"Project with ID {projectId} not found");

        if (project.ClientId != clientId)
            throw new ForbiddenException("You are not authorized to update this project");

        if (project.Status != ProjectStatus.Draft)
            throw new BadRequestException("Only draft projects can be updated");

        if (request.Title != null) project.Title = request.Title;
        if (request.Description != null) project.Description = request.Description;
        if (request.Category != null) project.Category = request.Category;
        if (request.Skills != null) project.Skills = request.Skills;
        if (request.Budget.HasValue) project.Budget = request.Budget.Value;
        if (request.Currency != null) project.Currency = request.Currency;
        if (request.Deadline.HasValue) project.Deadline = request.Deadline;
        if (request.EstimatedDuration.HasValue) project.EstimatedDuration = request.EstimatedDuration.Value;
        if (request.DurationUnit != null) project.DurationUnit = request.DurationUnit;

        if (request.RequiredExperienceLevel != null)
        {
            if (!Enum.TryParse<ExperienceLevel>(request.RequiredExperienceLevel, ignoreCase: true, out var level))
                throw new ValidationException(new[] { $"Invalid experience level: '{request.RequiredExperienceLevel}'" });
            project.RequiredExperienceLevel = level;
        }

        await _unitOfWork.Projects.UpdateAsync(project);
        await _unitOfWork.SaveChangesAsync();

        var client = await _unitOfWork.Users.GetByIdAsync(clientId);
        return MapToDto(project, client);
    }

    public async Task<bool> DeleteProjectAsync(Guid projectId, Guid clientId)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new NotFoundException($"Project with ID {projectId} not found");

        if (project.ClientId != clientId)
            throw new ForbiddenException("You are not authorized to delete this project");

        if (project.Status == ProjectStatus.InProgress)
            throw new BadRequestException("Cannot delete a project that is currently in progress");

        _unitOfWork.Projects.Remove(project);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> PublishProjectAsync(Guid projectId, Guid clientId)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new NotFoundException($"Project with ID {projectId} not found");

        if (project.ClientId != clientId)
            throw new ForbiddenException("You are not authorized to publish this project");

        if (project.Status != ProjectStatus.Draft)
            throw new BadRequestException("Only draft projects can be published");

        project.Status = ProjectStatus.Open;
        project.PublishedAt = DateTime.UtcNow;

        await _unitOfWork.Projects.UpdateAsync(project);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CloseProjectAsync(Guid projectId, Guid clientId)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new NotFoundException($"Project with ID {projectId} not found");

        if (project.ClientId != clientId)
            throw new ForbiddenException("You are not authorized to close this project");

        if (project.Status == ProjectStatus.Completed || project.Status == ProjectStatus.Cancelled)
            throw new BadRequestException("Project is already closed");

        project.Status = ProjectStatus.Cancelled;
        project.ClosedAt = DateTime.UtcNow;

        await _unitOfWork.Projects.UpdateAsync(project);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AwardProjectAsync(Guid projectId, Guid clientId, Guid freelancerId)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new NotFoundException($"Project with ID {projectId} not found");

        if (project.ClientId != clientId)
            throw new ForbiddenException("You are not authorized to award this project");

        if (project.Status != ProjectStatus.Open)
            throw new BadRequestException("Only open projects can be awarded");

        var bids = await _unitOfWork.Bids.GetByProjectIdAsync(projectId);
        var winningBid = bids.FirstOrDefault(b => b.FreelancerId == freelancerId && b.Status == BidStatus.Pending);

        if (winningBid == null)
            throw new NotFoundException("No pending bid found for this freelancer on this project");

        project.AwardedFreelancerId = freelancerId;
        project.AwardedAt = DateTime.UtcNow;
        project.Status = ProjectStatus.InProgress;

        winningBid.Status = BidStatus.Accepted;
        winningBid.AcceptedAt = DateTime.UtcNow;

        await _unitOfWork.Projects.UpdateAsync(project);
        await _unitOfWork.Bids.UpdateAsync(winningBid);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static ProjectDto MapToDto(Project project, User? client) => new()
    {
        Id = project.Id,
        ClientId = project.ClientId,
        ClientName = client != null ? $"{client.FirstName} {client.LastName}" : string.Empty,
        Title = project.Title,
        Description = project.Description,
        Category = project.Category,
        Skills = project.Skills,
        Status = project.Status.ToString(),
        Type = project.Type.ToString(),
        Budget = project.Budget,
        Currency = project.Currency,
        Deadline = project.Deadline,
        EstimatedDuration = project.EstimatedDuration,
        DurationUnit = project.DurationUnit,
        RequiredExperienceLevel = project.RequiredExperienceLevel.ToString(),
        ViewsCount = project.ViewsCount,
        BidsCount = project.BidsCount,
        PublishedAt = project.PublishedAt,
        CreatedAt = project.CreatedAt
    };
}
