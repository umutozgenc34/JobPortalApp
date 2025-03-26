using Microsoft.AspNetCore.Http;

namespace JobPortalApp.Model.Companies.Dtos;

public sealed record UpdateCompanyRequest(
    int Id,
    string Name,
    string About,
    string Industry,
    IFormFile LogoUrl,
    string? Email,
    string? PhoneNumber,
    string? WebsiteUrl,
    string? Location
);