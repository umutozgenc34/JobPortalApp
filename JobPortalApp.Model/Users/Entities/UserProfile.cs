using JobPortalApp.Shared.Entities;

namespace JobPortalApp.Model.Users.Entities;

public class UserProfile : BaseEntity<int>
{
    public string? FullName { get; set; }
    public string? ProfileImageUrl { get; set; }

    public string? Summary { get; set; }
    public string? Email { get; set; }
    public string? CvFilePath { get; set; }
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
}
