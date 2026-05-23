using FreelanceHub.Application.DTOs.Bids;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Interfaces;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Interfaces;

namespace FreelanceHub.Application.Services.Implementations;

public class BidService : IBidService
{
    private readonly IUnitOfWork _unitOfWork;

    public BidService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BidDto> SubmitBidAsync(Guid freelancerId, CreateBidRequest request)
    {
        var freelancer = await _unitOfWork.Users.GetByIdAsync(freelancerId)
            ?? throw new NotFoundException("Freelancer not found");

        if (freelancer.Role != UserRole.Freelancer)
            throw new ForbiddenException("Only freelancers can submit bids");

        var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId)
            ?? throw new NotFoundException($"Project with ID {request.ProjectId} not found");

        if (project.Status != ProjectStatus.Open)
            throw new BadRequestException("Bids can only be submitted for open projects");

        if (project.ClientId == freelancerId)
            throw new BadRequestException("You cannot bid on your own project");

        var existingBid = await _unitOfWork.Bids.GetByProjectAndFreelancerAsync(request.ProjectId, freelancerId);
        if (existingBid != null && existingBid.Status != BidStatus.Withdrawn)
            throw new ConflictException("You have already submitted a bid for this project");

        var bid = new Bid
        {
            ProjectId = request.ProjectId,
            FreelancerId = freelancerId,
            Amount = request.Amount,
            Currency = request.Currency,
            DeliveryTime = request.DeliveryTime,
            DeliveryTimeUnit = request.DeliveryTimeUnit,
            CoverLetter = request.CoverLetter,
            Status = BidStatus.Pending
        };

        await _unitOfWork.Bids.AddAsync(bid);

        // Increment bid count on project
        project.BidsCount++;
        await _unitOfWork.Projects.UpdateAsync(project);

        await _unitOfWork.SaveChangesAsync();

        return MapToDto(bid, project, freelancer);
    }

    public async Task<BidDto> GetBidByIdAsync(Guid bidId)
    {
        var bid = await _unitOfWork.Bids.GetByIdAsync(bidId)
            ?? throw new NotFoundException($"Bid with ID {bidId} not found");

        var project = await _unitOfWork.Projects.GetByIdAsync(bid.ProjectId);
        var freelancer = await _unitOfWork.Users.GetByIdAsync(bid.FreelancerId);

        return MapToDto(bid, project, freelancer);
    }

    public async Task<IEnumerable<BidDto>> GetBidsByProjectAsync(Guid projectId)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new NotFoundException($"Project with ID {projectId} not found");

        var bids = await _unitOfWork.Bids.GetByProjectIdAsync(projectId);

        var dtos = new List<BidDto>();
        foreach (var bid in bids)
        {
            var freelancer = await _unitOfWork.Users.GetByIdAsync(bid.FreelancerId);
            dtos.Add(MapToDto(bid, project, freelancer));
        }

        return dtos;
    }

    public async Task<IEnumerable<BidDto>> GetMyBidsAsync(Guid freelancerId)
    {
        var freelancer = await _unitOfWork.Users.GetByIdAsync(freelancerId)
            ?? throw new NotFoundException("Freelancer not found");

        var bids = await _unitOfWork.Bids.GetByFreelancerIdAsync(freelancerId);

        var dtos = new List<BidDto>();
        foreach (var bid in bids)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(bid.ProjectId);
            dtos.Add(MapToDto(bid, project, freelancer));
        }

        return dtos;
    }

    public async Task<BidDto> UpdateBidAsync(Guid bidId, Guid freelancerId, UpdateBidRequest request)
    {
        var bid = await _unitOfWork.Bids.GetByIdAsync(bidId)
            ?? throw new NotFoundException($"Bid with ID {bidId} not found");

        if (bid.FreelancerId != freelancerId)
            throw new ForbiddenException("You are not authorized to update this bid");

        if (bid.Status != BidStatus.Pending)
            throw new BadRequestException("Only pending bids can be updated");

        if (request.Amount.HasValue) bid.Amount = request.Amount.Value;
        if (request.DeliveryTime.HasValue) bid.DeliveryTime = request.DeliveryTime.Value;
        if (request.DeliveryTimeUnit != null) bid.DeliveryTimeUnit = request.DeliveryTimeUnit;
        if (request.CoverLetter != null) bid.CoverLetter = request.CoverLetter;

        await _unitOfWork.Bids.UpdateAsync(bid);
        await _unitOfWork.SaveChangesAsync();

        var project = await _unitOfWork.Projects.GetByIdAsync(bid.ProjectId);
        var freelancer = await _unitOfWork.Users.GetByIdAsync(freelancerId);

        return MapToDto(bid, project, freelancer);
    }

    public async Task<bool> WithdrawBidAsync(Guid bidId, Guid freelancerId)
    {
        var bid = await _unitOfWork.Bids.GetByIdAsync(bidId)
            ?? throw new NotFoundException($"Bid with ID {bidId} not found");

        if (bid.FreelancerId != freelancerId)
            throw new ForbiddenException("You are not authorized to withdraw this bid");

        if (bid.Status != BidStatus.Pending)
            throw new BadRequestException("Only pending bids can be withdrawn");

        bid.Status = BidStatus.Withdrawn;

        var project = await _unitOfWork.Projects.GetByIdAsync(bid.ProjectId);
        if (project != null && project.BidsCount > 0)
        {
            project.BidsCount--;
            await _unitOfWork.Projects.UpdateAsync(project);
        }

        await _unitOfWork.Bids.UpdateAsync(bid);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AcceptBidAsync(Guid bidId, Guid clientId)
    {
        var bid = await _unitOfWork.Bids.GetByIdAsync(bidId)
            ?? throw new NotFoundException($"Bid with ID {bidId} not found");

        var project = await _unitOfWork.Projects.GetByIdAsync(bid.ProjectId)
            ?? throw new NotFoundException("Project not found");

        if (project.ClientId != clientId)
            throw new ForbiddenException("You are not authorized to accept bids on this project");

        if (bid.Status != BidStatus.Pending)
            throw new BadRequestException("Only pending bids can be accepted");

        bid.Status = BidStatus.Accepted;
        bid.AcceptedAt = DateTime.UtcNow;

        project.AwardedFreelancerId = bid.FreelancerId;
        project.AwardedAt = DateTime.UtcNow;
        project.Status = ProjectStatus.InProgress;

        await _unitOfWork.Bids.UpdateAsync(bid);
        await _unitOfWork.Projects.UpdateAsync(project);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RejectBidAsync(Guid bidId, Guid clientId, string reason)
    {
        var bid = await _unitOfWork.Bids.GetByIdAsync(bidId)
            ?? throw new NotFoundException($"Bid with ID {bidId} not found");

        var project = await _unitOfWork.Projects.GetByIdAsync(bid.ProjectId)
            ?? throw new NotFoundException("Project not found");

        if (project.ClientId != clientId)
            throw new ForbiddenException("You are not authorized to reject bids on this project");

        if (bid.Status != BidStatus.Pending)
            throw new BadRequestException("Only pending bids can be rejected");

        bid.Status = BidStatus.Rejected;
        bid.RejectedAt = DateTime.UtcNow;
        bid.RejectionReason = reason;

        await _unitOfWork.Bids.UpdateAsync(bid);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static BidDto MapToDto(Bid bid, Project? project, User? freelancer) => new()
    {
        Id = bid.Id,
        ProjectId = bid.ProjectId,
        ProjectTitle = project?.Title ?? string.Empty,
        FreelancerId = bid.FreelancerId,
        FreelancerName = freelancer != null ? $"{freelancer.FirstName} {freelancer.LastName}" : string.Empty,
        Amount = bid.Amount,
        Currency = bid.Currency,
        DeliveryTime = bid.DeliveryTime,
        DeliveryTimeUnit = bid.DeliveryTimeUnit,
        CoverLetter = bid.CoverLetter,
        Status = bid.Status.ToString(),
        AcceptedAt = bid.AcceptedAt,
        CreatedAt = bid.CreatedAt
    };
}
