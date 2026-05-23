using FreelanceHub.Application.DTOs.Common;
using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Application.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<PagedResponse<User>> GetAllAsync(int pageNumber, int pageSize);
    Task<bool> UpdateProfileAsync(Guid userId, object updateDto);
    Task<bool> DeleteAccountAsync(Guid userId);
}
