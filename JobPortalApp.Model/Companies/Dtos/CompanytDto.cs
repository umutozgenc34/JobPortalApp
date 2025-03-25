namespace JobPortalApp.Model.Companies.Dtos;

public sealed record CompanytDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string About { get; init; }
    public string Industry { get; init; }
    public string LogoUrl { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? WebsiteUrl { get; init; }
    public string? Location { get; init; }
}
