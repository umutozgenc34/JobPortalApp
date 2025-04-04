namespace JobPortalApp.Model.Users.Dtos;

public sealed record UserProfileDto
{
    public int Id { get; init; }
    public string FullName { get; init; } 
    public string? ProfileImageUrl { get; init; }
    public string? Summary { get; init; }
    public string Email { get; init; } 
    public string? CvFilePath { get; init; }
    public string UserName { get; init; } 
}