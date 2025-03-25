namespace JobPortalApp.Model.Companies.Dtos;

public sealed record UpdateCompanyRequest(
    int Id,
    string Name,
    string About,
    string Industry,
    string LogoUrl,
    string? Email,
    string? PhoneNumber,
    string? WebsiteUrl,
    string? Location
);