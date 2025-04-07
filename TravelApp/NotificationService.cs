using Microsoft.EntityFrameworkCore;
using Shared.Models;
using TravelApp.Entities;

namespace TravelApp;

public class NotificationService
{
    private static readonly HttpClient _httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7226")
    };

    public async Task ReminderUsersAsync()
    {
        var topBookingDate = TravelDbContext.Instance.Bookings
            .GroupBy(b => b.TravelDate)
            .Select(g => new
            {
                TravelDate = g.Key,
                TotalBookings = g.Count()
            })
            .OrderByDescending(x => x.TotalBookings)
            .FirstOrDefault();

        var bookings = await TravelDbContext.Instance.Bookings
            .Where(b => b.TravelDate == topBookingDate.TravelDate)
            .Select(b => new
            {
                UserId = b.UserId,
                TravelDate = b.TravelDate,
                UserEmail = b.User.Email,
                UserPhoneNumber = b.User.PhoneNumber,
                FlightFlightNumber = b.Flight.FlightNumber,
                FlightScheduledDepartureTime = b.Flight.ScheduledDepartureTime
            })
            .ToListAsync();

        foreach (var booking in bookings)
        {
            string message = $"Reminder: You have a booking for flight {booking.FlightFlightNumber} on {booking.FlightScheduledDepartureTime}.";
            var user = new User
            {
                Email = booking.UserEmail,
                PhoneNumber = booking.UserPhoneNumber,
                Id = booking.UserId
            };
            var resp = await SendNotificationAsync(user, message);
        }
    }

    public async Task<SendNotificationResponse> SendNotificationAsync(User user, string message)
    {
        var httpResp = await _httpClient.PostAsJsonAsync("/api/Notification", new SendNotificationRequest
        {
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Message = message,
            UserId = user.Id
        });

        var responseDto = await httpResp.Content.ReadFromJsonAsync<SendNotificationResponse>();
        return responseDto;
    }
}
