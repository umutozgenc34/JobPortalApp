using AutoMapper;
using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Repository.Users.Abstracts;
using JobPortalApp.Service.Users.Abstracts;
using JobPortalApp.Shared.Responses;
using JobPortalApp.Shared.Services.Cloudinaryy.Abstracts;
using System.Net;

namespace JobPortalApp.Service.Users.Concretes;

public class UserProfileService(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICloudinaryService cloudinaryService
) : IUserProfileService
{
    public async Task<ServiceResult<UserProfileDto>> CreateAsync(CreateUserProfileRequest request)
    {
        var profile = mapper.Map<UserProfile>(request);

        if (request.ProfileImage is not null)
        {
            var imageUrl = await cloudinaryService.UploadImage(request.ProfileImage, "profile_images");
            profile.ProfileImageUrl = imageUrl;
        }

        if (request.CvFile is not null)
        {
            var cvUrl = await cloudinaryService.UploadPdf(request.CvFile, "cv_files");
            profile.CvFilePath = cvUrl;
        }

        await userProfileRepository.AddAsync(profile);
        await unitOfWork.SaveChangesAsync();

        var dto = mapper.Map<UserProfileDto>(profile);
        return ServiceResult<UserProfileDto>.SuccessAsCreated(dto, $"userprofiles/{profile.Id}");
    }

    public async Task<ServiceResult<UserProfileDto>> GetByIdAsync(int id)
    {
        var profile = await userProfileRepository.GetByIdAsync(id);
        if (profile is null)
        {
            return ServiceResult<UserProfileDto>.Fail("Profile not found.", HttpStatusCode.NotFound);
        }

        var dto = mapper.Map<UserProfileDto>(profile);
        return ServiceResult<UserProfileDto>.Success(dto);
    }

    public async Task<ServiceResult<List<UserProfileDto>>> GetAllAsync()
    {
        var profiles = await userProfileRepository.GetAllAsync();
        var dtoList = mapper.Map<List<UserProfileDto>>(profiles);
        return ServiceResult<List<UserProfileDto>>.Success(dtoList);
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateUserProfileRequest request)
    {
        var profile = await userProfileRepository.GetByIdAsync(id);
        if (profile is null)
        {
            return ServiceResult.Fail("Profile not found.", HttpStatusCode.NotFound);
        }

        mapper.Map(request, profile);

        if (request.ProfileImage is not null)
        {
            var imageUrl = await cloudinaryService.UploadImage(request.ProfileImage, "profile_images");
            profile.ProfileImageUrl = imageUrl;
        }

        if (request.CvFile is not null)
        {
            var cvUrl = await cloudinaryService.UploadPdf(request.CvFile, "cv_files");
            profile.CvFilePath = cvUrl;
        }

        userProfileRepository.Update(profile);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success("Profile updated successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var profile = await userProfileRepository.GetByIdAsync(id);
        if (profile is null)
        {
            return ServiceResult.Fail("Profile not found.", HttpStatusCode.NotFound);
        }

        userProfileRepository.Delete(profile);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success("Profile deleted successfully.", HttpStatusCode.NoContent);
    }
}
