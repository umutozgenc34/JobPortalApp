using Microsoft.AspNetCore.Http;

namespace JobPortalApp.Shared.Services.Cloudinaryy.Abstracts;

public interface ICloudinaryService
{
    Task<string> UploadImage(IFormFile formFile, string imageDirectory);
}