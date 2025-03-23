using JobPortalApp.Shared.Entities;

namespace JobPortalApp.Model.Categories.Entities;

public class Category : BaseEntity<int>
{
    public string Name { get; set; } = default!;
}
