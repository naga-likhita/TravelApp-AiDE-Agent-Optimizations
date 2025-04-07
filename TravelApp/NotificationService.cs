﻿using Microsoft.EntityFrameworkCore;
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

        foreach (var booking in bookings)
        {
            if (booking.User != null)
            {
                string message = $"Reminder: You have a booking for flight {booking.Flight.FlightNumber} on {booking.Flight.ScheduledDepartureTime}.";
                var resp = await SendNotificationAsync(booking.User, message);
            }
        }
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
