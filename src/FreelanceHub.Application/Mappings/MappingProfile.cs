using AutoMapper;
using FreelanceHub.Application.DTOs.Auth;
using FreelanceHub.Application.DTOs.Bids;
using FreelanceHub.Application.DTOs.Messages;
using FreelanceHub.Application.DTOs.Notifications;
using FreelanceHub.Application.DTOs.Payments;
using FreelanceHub.Application.DTOs.Projects;
using FreelanceHub.Application.DTOs.Reviews;
using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        CreateMap<RegisterRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Project mappings
        CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src =>
                src.Client != null ? $"{src.Client.FirstName} {src.Client.LastName}" : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.RequiredExperienceLevel, opt => opt.MapFrom(src => src.RequiredExperienceLevel.ToString()));

        // Bid mappings
        CreateMap<Bid, BidDto>()
            .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src =>
                src.Project != null ? src.Project.Title : string.Empty))
            .ForMember(dest => dest.FreelancerName, opt => opt.MapFrom(src =>
                src.Freelancer != null ? $"{src.Freelancer.FirstName} {src.Freelancer.LastName}" : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // Message mappings
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src =>
                src.Sender != null ? $"{src.Sender.FirstName} {src.Sender.LastName}" : string.Empty))
            .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src =>
                src.Receiver != null ? $"{src.Receiver.FirstName} {src.Receiver.LastName}" : string.Empty))
            .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src =>
                src.Project != null ? src.Project.Title : null));

        // Payment mappings
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src =>
                src.Project != null ? src.Project.Title : string.Empty))
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src =>
                src.Sender != null ? $"{src.Sender.FirstName} {src.Sender.LastName}" : string.Empty))
            .ForMember(dest => dest.ReceiverName, opt => opt.MapFrom(src =>
                src.Receiver != null ? $"{src.Receiver.FirstName} {src.Receiver.LastName}" : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        // Notification mappings
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        // Review mappings
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src =>
                src.Project != null ? src.Project.Title : string.Empty))
            .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src =>
                src.Reviewer != null ? $"{src.Reviewer.FirstName} {src.Reviewer.LastName}" : string.Empty))
            .ForMember(dest => dest.RevieweeName, opt => opt.MapFrom(src =>
                src.Reviewee != null ? $"{src.Reviewee.FirstName} {src.Reviewee.LastName}" : string.Empty));
    }
}
