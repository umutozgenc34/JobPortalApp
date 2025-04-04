namespace JobPortalApp.Model.Users.Dtos;

public sealed record UserWithProfileDto
{
    public string Id { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }

    public UserProfileDto Profile { get; init; }

}
