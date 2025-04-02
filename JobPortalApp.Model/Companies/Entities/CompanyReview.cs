using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Shared.Entities;

namespace JobPortalApp.Model.Companies.Entities;

public class CompanyReview : BaseEntity<int>, IAuditEntity
{
    public int Rating { get; set; }
    public string Comment { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public string UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
