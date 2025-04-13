using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Shared.Entities;

namespace JobPortalApp.Model.Notifications.Entities;

public class Notification : BaseEntity<Guid>, IAuditEntity
{
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set ; }
}
