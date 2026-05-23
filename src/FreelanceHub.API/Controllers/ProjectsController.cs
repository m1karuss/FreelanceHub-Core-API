using FreelanceHub.Application.DTOs.Common;
using FreelanceHub.Application.DTOs.Projects;
using FreelanceHub.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreelanceHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Get all open projects with optional search and filters
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ProjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjects(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] decimal? minBudget,
        [FromQuery] decimal? maxBudget,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _projectService.GetProjectsAsync(search, category, minBudget, maxBudget, pageNumber, pageSize);
        return Ok(ApiResponse<PagedResponse<ProjectDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get a project by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject([FromRoute] Guid id)
    {
        var result = await _projectService.GetProjectByIdAsync(id);
        return Ok(ApiResponse<ProjectDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Get my projects (client only)
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyProjects()
    {
        var result = await _projectService.GetMyProjectsAsync(CurrentUserId);
        return Ok(ApiResponse<IEnumerable<ProjectDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Create a new project (client only)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        _logger.LogInformation("Creating project for client {ClientId}", CurrentUserId);

        var result = await _projectService.CreateProjectAsync(CurrentUserId, request);

        return CreatedAtAction(nameof(GetProject), new { id = result.Id },
            ApiResponse<ProjectDto>.SuccessResponse(result, "Project created successfully"));
    }

    /// <summary>
    /// Update a project (owner only, draft only)
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProject([FromRoute] Guid id, [FromBody] UpdateProjectRequest request)
    {
        var result = await _projectService.UpdateProjectAsync(id, CurrentUserId, request);
        return Ok(ApiResponse<ProjectDto>.SuccessResponse(result, "Project updated successfully"));
    }

    /// <summary>
    /// Delete a project (owner only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid id)
    {
        var result = await _projectService.DeleteProjectAsync(id, CurrentUserId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Project deleted successfully"));
    }

    /// <summary>
    /// Publish a project (moves from Draft to Open)
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PublishProject([FromRoute] Guid id)
    {
        var result = await _projectService.PublishProjectAsync(id, CurrentUserId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Project published successfully"));
    }

    /// <summary>
    /// Close a project
    /// </summary>
    [HttpPost("{id:guid}/close")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CloseProject([FromRoute] Guid id)
    {
        var result = await _projectService.CloseProjectAsync(id, CurrentUserId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Project closed successfully"));
    }

    /// <summary>
    /// Award a project to a freelancer
    /// </summary>
    [HttpPost("{id:guid}/award/{freelancerId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AwardProject([FromRoute] Guid id, [FromRoute] Guid freelancerId)
    {
        var result = await _projectService.AwardProjectAsync(id, CurrentUserId, freelancerId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Project awarded successfully"));
    }
}
