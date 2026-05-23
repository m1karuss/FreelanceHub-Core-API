using FreelanceHub.Application.Services.Interfaces;

namespace FreelanceHub.Application.Services.Implementations;

public class UserService : IUserService
{
    // Implementation will be added
    public Task<Domain.Entities.User?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Entities.User?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<DTOs.Common.PagedResponse<Domain.Entities.User>> GetAllAsync(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateProfileAsync(Guid userId, object updateDto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAccountAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}
