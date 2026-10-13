using Microsoft.AspNetCore.Http;

namespace JobPortalApp.Model.Companies.Dtos;

public sealed record UpdateCompanyRequest(
    string Name,
    string About,
    string Industry,
    IFormFile? Logo,
    string? Email,
    string? PhoneNumber,
    string? WebsiteUrl,
    string? Location
);
