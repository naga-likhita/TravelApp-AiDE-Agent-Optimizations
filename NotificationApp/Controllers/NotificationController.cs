using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace NotificationApp.Controllers;
[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    [HttpPost]
    public async Task<SendNotificationResponse> SendNotification(SendNotificationRequest request)
    {
        await Task.Delay(100);
        return new SendNotificationResponse()
        {
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Message = request.Message,
            SentAt = DateTime.Now,
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Status = "SENT"
        };
    }
}
