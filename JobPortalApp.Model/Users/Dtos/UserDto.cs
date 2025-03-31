namespace JobPortalApp.Model.Users.Dtos;

public sealed record UserDto
{
    public string Id { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
};