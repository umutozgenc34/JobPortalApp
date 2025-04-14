namespace JobPortalApp.Model.Notifications.Dtos;

public sealed record NotificationDto

{
    public Guid Id { get; init; }
    public string UserId { get; init; }
    public string Title { get; init; }
    public string Message { get; init; }
    public bool IsRead { get; init; }
    public DateTime CreatedAt { get; init; }
}
