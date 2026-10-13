namespace JobPortalApp.Model.Users.Dtos;

public sealed record CurrentUserDto
{
    public string Id { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
    public List<string> Roles { get; init; } = [];
}
