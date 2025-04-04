using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Shared.Responses;

namespace JobPortalApp.Service.Users.Abstracts;

public interface IUserProfileService
{
    Task<ServiceResult<UserProfileDto>> CreateAsync(CreateUserProfileRequest request);
    Task<ServiceResult> UpdateAsync(int id, UpdateUserProfileRequest request);
    Task<ServiceResult> DeleteAsync(int id);
    Task<ServiceResult<UserProfileDto>> GetByIdAsync(int id);
    Task<ServiceResult<List<UserProfileDto>>> GetAllAsync();
}