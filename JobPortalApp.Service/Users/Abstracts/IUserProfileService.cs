using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Shared.Responses;

namespace JobPortalApp.Service.Users.Abstracts;

public interface IUserProfileService
{
    Task<ServiceResult<UserProfileDto>> CreateAsync(CreateUserProfileRequest request, string userId);
    Task<ServiceResult> UpdateAsync(string userId, UpdateUserProfileRequest request);
    Task<ServiceResult> DeleteAsync(string userId);
    Task<ServiceResult<UserProfileDto>> GetByIdAsync(int id);
    Task<ServiceResult<UserProfileDto>> GetByUserIdAsync(string userId);
    Task<ServiceResult<List<UserProfileDto>>> GetAllAsync();
}
