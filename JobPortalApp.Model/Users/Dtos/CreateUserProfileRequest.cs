using Microsoft.AspNetCore.Http;

namespace JobPortalApp.Model.Users.Dtos;

public sealed record CreateUserProfileRequest(string FullName,string Summary,string Email,IFormFile? ProfileImage,IFormFile? CvFile,string UserId);
