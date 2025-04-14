using JobPortalApp.Service.Notifications.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

//[Authorize]
public class NotificationsController(INotificationService notificationService) : CustomBaseController
{
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserNotifications([FromRoute] string userId)
        => CreateActionResult(await notificationService.GetUserNotificationAsync(userId));
}