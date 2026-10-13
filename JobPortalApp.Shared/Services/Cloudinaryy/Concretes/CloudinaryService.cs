using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using JobPortalApp.Shared.Services.Cloudinaryy.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using JobPortalApp.Shared.Services.Cloudinaryy.Settings;

namespace JobPortalApp.Shared.Services.Cloudinaryy.Concretes;

public sealed class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> cloudOptions)
    {
        var settings = cloudOptions.Value;
        var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImage(IFormFile formFile, string imageDirectory)
    {
        if (formFile.Length == 0) return string.Empty;

        using var stream = formFile.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(formFile.FileName, stream),
            Folder = imageDirectory
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        if (uploadResult.Error is not null)
        {
            throw new InvalidOperationException($"Image upload failed: {uploadResult.Error.Message}");
        }

        return uploadResult.SecureUrl.ToString();
    }

    public async Task<string> UploadPdf(IFormFile formFile, string folderName)
    {
        if (formFile.Length == 0) return string.Empty;

        using var stream = formFile.OpenReadStream();
        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(formFile.FileName, stream),
            Folder = folderName
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        if (uploadResult.Error is not null)
        {
            throw new InvalidOperationException($"File upload failed: {uploadResult.Error.Message}");
        }

        return uploadResult.SecureUrl.ToString();
    }
}
