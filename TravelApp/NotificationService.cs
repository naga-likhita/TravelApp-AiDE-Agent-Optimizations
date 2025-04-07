using Microsoft.EntityFrameworkCore;
using Shared.Models;
using TravelApp.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;

namespace TravelApp;

public class NotificationService
{
    private static readonly HttpClient _httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7226")
    };

    private readonly IMemoryCache _memoryCache;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public NotificationService(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
    {
        _memoryCache = memoryCache;
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task ReminderUsersAsync()
    {
        var cacheKeyTopBookingDate = "top_booking_date";

        if (!_memoryCache.TryGetValue(cacheKeyTopBookingDate, out DateOnly topBookingDate))
        {
            var topBookingDateResult = TravelDbContext.Instance.Bookings
                .GroupBy(b => b.TravelDate)
                .Select(g => new
                {
                    TravelDate = g.Key,
                    TotalBookings = g.Count()
                })
                .OrderByDescending(x => x.TotalBookings)
                .FirstOrDefault();

            topBookingDate = topBookingDateResult?.TravelDate ?? DateOnly.MinValue;

            _memoryCache.Set(cacheKeyTopBookingDate, topBookingDate, TimeSpan.FromMinutes(10)); // Cache for 10 minutes
        }

        var bookings = await TravelDbContext.Instance.Bookings
    .Where(b => b.TravelDate == topBookingDate)
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

        var notificationTasks = bookings.Select(booking =>
        {
            string message = $"Reminder: You have a booking for flight {booking.FlightFlightNumber} on {booking.FlightScheduledDepartureTime}.";
            var user = new User
            {
                Email = booking.UserEmail,
                PhoneNumber = booking.UserPhoneNumber,
                Id = booking.UserId
            };
            return SendNotificationAsync(user, message);
        }).ToArray();

        await Task.WhenAll(notificationTasks);
    }

    public async Task<SendNotificationResponse> SendNotificationAsync(User user, string message)
    {
        var correlationId = _httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();
        _httpClient.DefaultRequestHeaders.Add("X-Correlation-ID", correlationId);

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
