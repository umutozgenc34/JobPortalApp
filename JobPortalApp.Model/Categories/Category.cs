using JobPortalApp.Shared.Entities;

namespace JobPortalApp.Model.Categories;

public class Category : BaseEntity<int>
{
    public string Name { get; set; } = default!;
}
