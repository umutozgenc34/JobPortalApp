using Microsoft.AspNetCore.Http;

namespace JobPortalApp.Model.Users.Dtos;

public sealed record UpdateUserProfileRequest(string FullName, string Summary, string Email, IFormFile? ProfileImage, IFormFile? CvFile);
