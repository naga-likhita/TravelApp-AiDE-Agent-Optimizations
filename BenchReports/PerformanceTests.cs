﻿using BenchmarkDotNet.Attributes;
using TravelApp;
using TravelApp.DTOs;
using TravelApp.Entities;

namespace BenchReports;


[MemoryDiagnoser]
public class PerformanceTests
{
    [Benchmark]
    public async Task<List<Flight>> GetFlights()
    {
        return await TravelRepo.Instance.GetFlightsAsync("Goa", "Hyderabad");
    }

    [Benchmark]
    public async Task<decimal> GetFlightSpeed()
    {
        return await TravelRepo.Instance.GetFlightSpeedAsync(25);
    }

    [Benchmark]
    public async Task<List<Booking>> GetBookings()
    {
        return await TravelRepo.Instance.GetBookings();
    }

    [Benchmark]
    public async Task<List<Booking>> GetBookingsForUser()
    {
        return await TravelRepo.Instance.GetBookingsForUser("myra.jain@mail.com");
    }

    [Benchmark]
    public async Task<UserInfoDto> GetUserInfoByEmail()
    {
        return await TravelRepo.Instance.GetUserInfoByEmail("navya.gupta@mail.com");
    }

    [Benchmark]
    public async Task<UserInfoDto[]> GetUsersByCountry()
    {
        return await TravelRepo.Instance.GetUsersByCountry("India");
    }

    [Benchmark]
    public void DataStoreFormat()
    {
        var user = TravelDbContext.Instance.Users.First();
        var newPrefs = user.Preferences;
        newPrefs.WindowSeatPreference = true;
        newPrefs.WantsTelevision = true;
        newPrefs.WantsMeal = true;
        newPrefs.SpecialAssistanceRequired = true;
        user.Preferences = newPrefs;
    }

    [Benchmark]
    public async Task SendNotification()
    {
        var user = new User()
        {
            Email = "Likhita@test.com",
            PhoneNumber = "9011112222",
            Id = 1,
        };

        var resp = await new NotificationService()
            .SendNotificationAsync(user, "Hi [UserName], this is a reminder for your upcoming flight from [Departure] to [Destination] on [BookingDate] at [DepartureTime]. \r\nPlease arrive at the airport at least 2 hours before departure. \r\nThank you for choosing [AirlineName]! Safe travels! ✈️\r\n");
    }

    [Benchmark]
    public async Task ReminderUsersAsync()
    {
        await new NotificationService().ReminderUsersAsync();
    }
}