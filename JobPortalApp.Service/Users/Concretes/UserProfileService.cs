using AutoMapper;
using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Repository.Users.Abstracts;
using JobPortalApp.Service.Users.Abstracts;
using JobPortalApp.Shared.Responses;
using JobPortalApp.Shared.Services.Cloudinaryy.Abstracts;
using Microsoft.Extensions.Logging;
using System.Net;

namespace JobPortalApp.Service.Users.Concretes;

public class UserProfileService(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICloudinaryService cloudinaryService,
    ILogger<UserProfileService> logger
) : IUserProfileService
{
    public async Task<ServiceResult<UserProfileDto>> CreateAsync(CreateUserProfileRequest request)
    {
        logger.LogInformation("CreateAsync called for UserProfile creation.");

        var profile = mapper.Map<UserProfile>(request);

        if (request.ProfileImage is not null)
        {
            var imageUrl = await cloudinaryService.UploadImage(request.ProfileImage, "profile_images");
            profile.ProfileImageUrl = imageUrl;
            logger.LogInformation("Profile image uploaded for UserProfile.");
        }

        if (request.CvFile is not null)
        {
            var cvUrl = await cloudinaryService.UploadPdf(request.CvFile, "cv_files");
            profile.CvFilePath = cvUrl;
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

        var profile = await userProfileRepository.GetByIdAsync(id);
        if (profile is null)
        {
            logger.LogWarning("UserProfile not found with Id: {Id}", id);
            return ServiceResult<UserProfileDto>.Fail("Profile not found.", HttpStatusCode.NotFound);
        }

        var dto = mapper.Map<UserProfileDto>(profile);
        logger.LogInformation("UserProfile retrieved successfully for Id: {Id}", id);
        return ServiceResult<UserProfileDto>.Success(dto);
    }

    public async Task<ServiceResult<List<UserProfileDto>>> GetAllAsync()
    {
        logger.LogInformation("GetAllAsync called for UserProfiles.");

        var profiles = await userProfileRepository.GetAllAsync();
        var dtoList = mapper.Map<List<UserProfileDto>>(profiles);

        logger.LogInformation("{Count} UserProfiles retrieved.", dtoList.Count);
        return ServiceResult<List<UserProfileDto>>.Success(dtoList);
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateUserProfileRequest request)
    {
        logger.LogInformation("UpdateAsync called for UserProfile with Id: {Id}", id);

        var profile = await userProfileRepository.GetByIdAsync(id);
        if (profile is null)
        {
            logger.LogWarning("Update failed - UserProfile not found with Id: {Id}", id);
            return ServiceResult.Fail("Profile not found.", HttpStatusCode.NotFound);
        }

        mapper.Map(request, profile);

        if (request.ProfileImage is not null)
        {
            var imageUrl = await cloudinaryService.UploadImage(request.ProfileImage, "profile_images");
            profile.ProfileImageUrl = imageUrl;
            logger.LogInformation("Profile image updated for UserProfile Id: {Id}", id);
        }

        if (request.CvFile is not null)
        {
            var cvUrl = await cloudinaryService.UploadPdf(request.CvFile, "cv_files");
            profile.CvFilePath = cvUrl;
            logger.LogInformation("CV file updated for UserProfile Id: {Id}", id);
        }

        userProfileRepository.Update(profile);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("UserProfile updated successfully for Id: {Id}", id);
        return ServiceResult.Success("Profile updated successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        logger.LogInformation("DeleteAsync called for UserProfile with Id: {Id}", id);

        var profile = await userProfileRepository.GetByIdAsync(id);
        if (profile is null)
        {
            logger.LogWarning("Delete failed - UserProfile not found with Id: {Id}", id);
            return ServiceResult.Fail("Profile not found.", HttpStatusCode.NotFound);
        }

        userProfileRepository.Delete(profile);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("UserProfile deleted successfully for Id: {Id}", id);
        return ServiceResult.Success("Profile deleted successfully.", HttpStatusCode.NoContent);
    }
}
