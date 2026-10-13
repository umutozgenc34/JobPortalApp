using JobPortalApp.Api.Extensions;
using JobPortalApp.Service.Notifications.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

[Authorize]
public class NotificationsController(INotificationService notificationService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
        => CreateActionResult(await notificationService.GetUserNotificationAsync(User.GetUserId()));

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead([FromRoute] Guid id)
        => CreateActionResult(await notificationService.MarkAsReadAsync(id, User.GetUserId()));

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
        => CreateActionResult(await notificationService.MarkAllAsReadAsync(User.GetUserId()));
}
