using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Shared.Models;
using TravelApp.DTOs;
using TravelApp.Entities;

namespace TravelApp;

public class NotificationService
{
    private static readonly HttpClient _httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7226")
    };
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(15);

    public NotificationService(IMemoryCache cache)
    {
        _cache = cache;
    }
    public async Task ReminderUsersAsync()
    {
        var cacheKey = "TopTravelDate";
        if (!_cache.TryGetValue(cacheKey, out DateOnly topTravelDate))
        {
            topTravelDate = TravelDbContext.Instance.Bookings
                .GroupBy(b => b.TravelDate)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Select(x => x.Date)
                .FirstOrDefault();

            _cache.Set(cacheKey, topTravelDate, _cacheDuration);
        }

        var bookings = await TravelDbContext.Instance.Bookings
            .Where(b => b.TravelDate == topTravelDate)
            .Take(1000)
            .Select(b => new
            {
                User = new NotificationUserDto
                {
                    Email = b.User.Email,
                    PhoneNumber = b.User.PhoneNumber,
                    Id = b.User.Id
                },
                Flight = new
                {
                    b.Flight.FlightNumber,
                    b.Flight.ScheduledDepartureTime
                }
            })
            .AsNoTracking()
            .ToListAsync();

        var notificationTasks = bookings
            .Where(booking => booking.User != null)
            .Select(booking =>
            {
                string message = $"Reminder: You have a booking for flight {booking.Flight.FlightNumber} on {booking.Flight.ScheduledDepartureTime}.";
                return SendNotificationAsync(booking.User, message);
            });

        await Task.WhenAll(notificationTasks);
    }


    public async Task<SendNotificationResponse> SendNotificationAsync(
        NotificationUserDto user,
        string message)
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
