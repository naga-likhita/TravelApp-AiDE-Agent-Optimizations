namespace Shared.Models;

public class SendNotificationRequest
{
    public int UserId { get; set; }
    public string Message { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
