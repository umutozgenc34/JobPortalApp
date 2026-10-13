using AutoMapper;
using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Repository.Users.Abstracts;
using JobPortalApp.Service.Users.Abstracts;
using JobPortalApp.Shared.Responses;
using JobPortalApp.Shared.Services.Cloudinaryy.Abstracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace JobPortalApp.Service.Users.Concretes;

public class UserProfileService(
    IUserProfileRepository userProfileRepository,
    UserManager<User> userManager,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICloudinaryService cloudinaryService,
    ILogger<UserProfileService> logger
) : IUserProfileService
{
    public async Task<ServiceResult<UserProfileDto>> CreateAsync(CreateUserProfileRequest request, string userId)
    {
        logger.LogInformation("CreateAsync called for UserProfile creation. UserId: {UserId}", userId);

        var hasProfile = await userProfileRepository.Where(p => p.UserId == userId).AnyAsync();
        if (hasProfile)
        {
            logger.LogWarning("CreateAsync failed - UserId {UserId} already has a profile.", userId);
            return ServiceResult<UserProfileDto>.Fail("You already have a profile.", HttpStatusCode.Conflict);
        }

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return ServiceResult<UserProfileDto>.Fail("User not found.", HttpStatusCode.NotFound);
        }

        var profile = mapper.Map<UserProfile>(request);
        profile.UserId = userId;
        profile.Email = string.IsNullOrWhiteSpace(request.Email) ? user.Email : request.Email.Trim();

        if (request.ProfileImage is not null)
        {
            profile.ProfileImageUrl = await cloudinaryService.UploadImage(request.ProfileImage, "profile_images");
            logger.LogInformation("Profile image uploaded for UserProfile.");
        }

        if (request.CvFile is not null)
        {
            profile.CvFilePath = await cloudinaryService.UploadPdf(request.CvFile, "cv_files");
            logger.LogInformation("CV file uploaded for UserProfile.");
        }

        await userProfileRepository.AddAsync(profile);
        await unitOfWork.SaveChangesAsync();

        var dto = mapper.Map<UserProfileDto>(profile);
        logger.LogInformation("UserProfile created successfully with Id: {Id}", profile.Id);

        return ServiceResult<UserProfileDto>.SuccessAsCreated(dto, $"userprofiles/{profile.Id}");
    }

    public async Task<ServiceResult<UserProfileDto>> GetByIdAsync(int id)
    {
        logger.LogInformation("GetByIdAsync called for UserProfile with Id: {Id}", id);

        var profile = await userProfileRepository.Where(p => p.Id == id).AsNoTracking().FirstOrDefaultAsync();
        if (profile is null)
        {
            logger.LogWarning("UserProfile not found with Id: {Id}", id);
            return ServiceResult<UserProfileDto>.Fail("Profile not found.", HttpStatusCode.NotFound);
        }

        var dto = mapper.Map<UserProfileDto>(profile);
        return ServiceResult<UserProfileDto>.Success(dto);
    }

    public async Task<ServiceResult<UserProfileDto>> GetByUserIdAsync(string userId)
    {
        logger.LogInformation("GetByUserIdAsync called for UserId: {UserId}", userId);

        var profile = await userProfileRepository.Where(p => p.UserId == userId).AsNoTracking().FirstOrDefaultAsync();
        if (profile is null)
        {
            return ServiceResult<UserProfileDto>.Fail("Profile not found.", HttpStatusCode.NotFound);
        }

        var dto = mapper.Map<UserProfileDto>(profile);
        return ServiceResult<UserProfileDto>.Success(dto);
    }

    public async Task<ServiceResult<List<UserProfileDto>>> GetAllAsync()
    {
        logger.LogInformation("GetAllAsync called for UserProfiles.");

        var profiles = await userProfileRepository.Where().AsNoTracking().ToListAsync();
        var dtoList = mapper.Map<List<UserProfileDto>>(profiles);

        logger.LogInformation("{Count} UserProfiles retrieved.", dtoList.Count);
        return ServiceResult<List<UserProfileDto>>.Success(dtoList);
    }

    public async Task<ServiceResult> UpdateAsync(string userId, UpdateUserProfileRequest request)
    {
        logger.LogInformation("UpdateAsync called for UserProfile of UserId: {UserId}", userId);

        var profile = await userProfileRepository.Where(p => p.UserId == userId).FirstOrDefaultAsync();
        if (profile is null)
        {
            logger.LogWarning("Update failed - UserProfile not found for UserId: {UserId}", userId);
            return ServiceResult.Fail("Profile not found.", HttpStatusCode.NotFound);
        }

        var currentEmail = profile.Email;
        mapper.Map(request, profile);
        profile.Email = string.IsNullOrWhiteSpace(request.Email) ? currentEmail : request.Email.Trim();

        if (request.ProfileImage is not null)
        {
            profile.ProfileImageUrl = await cloudinaryService.UploadImage(request.ProfileImage, "profile_images");
            logger.LogInformation("Profile image updated for UserId: {UserId}", userId);
        }

        if (request.CvFile is not null)
        {
            profile.CvFilePath = await cloudinaryService.UploadPdf(request.CvFile, "cv_files");
            logger.LogInformation("CV file updated for UserId: {UserId}", userId);
        }

        userProfileRepository.Update(profile);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("UserProfile updated successfully for UserId: {UserId}", userId);
        return ServiceResult.Success("Profile updated successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(string userId)
    {
        logger.LogInformation("DeleteAsync called for UserProfile of UserId: {UserId}", userId);

        var profile = await userProfileRepository.Where(p => p.UserId == userId).FirstOrDefaultAsync();
        if (profile is null)
        {
            logger.LogWarning("Delete failed - UserProfile not found for UserId: {UserId}", userId);
            return ServiceResult.Fail("Profile not found.", HttpStatusCode.NotFound);
        }

        userProfileRepository.Delete(profile);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("UserProfile deleted successfully for UserId: {UserId}", userId);
        return ServiceResult.Success("Profile deleted successfully.", HttpStatusCode.NoContent);
    }
}
