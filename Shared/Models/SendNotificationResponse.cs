namespace Shared.Models;
public class SendNotificationResponse
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public int UserId { get; set; }
    public DateTime SentAt { get; set; }
}