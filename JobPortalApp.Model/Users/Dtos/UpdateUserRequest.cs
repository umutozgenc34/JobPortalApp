namespace JobPortalApp.Model.Users.Dtos;

public sealed record UpdateUserRequest(string Id,string UserName, string Email);