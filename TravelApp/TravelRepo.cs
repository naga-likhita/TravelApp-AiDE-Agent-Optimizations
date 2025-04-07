using Microsoft.EntityFrameworkCore;
using TravelApp.DTOs;
using TravelApp.Entities;

namespace TravelApp;

public class TravelRepo(TravelDbContext context)
{
    public static TravelRepo Instance => new(TravelDbContext.Instance);

    /// <summary>
    /// This API used too frequently to get list of flights
    /// </summary>
    /// <returns></returns>
    public async Task<List<Flight>> GetFlightsAsync(string departure, string destination)
    {
        return
            await context
            .Flights
            .Where(f => f.Departure == departure && f.Destination == destination).ToListAsync();
    }

    public async Task<decimal> GetFlightSpeedAsync(int flightId)
    {
        return await context.Flights
            .Where(f => f.Id == flightId)
            .Select(f => f.Speed)
            .FirstAsync(); // Already optimized - no full entity load
    }

    /// <summary>
    /// Large results as there will be many bookings in system
    /// </summary>
    /// <returns></returns>
    public async Task<List<Booking>> GetBookings()
    {
        return await context.Bookings.ToListAsync();
    }

    public async Task<List<Booking>> GetBookingsForUser(string userPhoneNum)
    {
        return await context.Bookings.Where(b => b.User.PhoneNumber == userPhoneNum).ToListAsync();
    }

    /// <summary>
    /// User visits profile page multiple times
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<UserInfoDto> GetUserInfoByEmail(string email)
    {
        return await context.Users.Where(u => u.Email == email)
            .Select(user => new UserInfoDto
            {
                Name = user.Name,
                Email = user.Email,
            }).FirstAsync();
    }

    public async Task<UserInfoDto[]> GetUsersByCountry(string country)
    {
        return await context.Users
            .Where(u => u.Country == country)
            .Select(u => new UserInfoDto
            {
                Name = u.Name,
                Email = u.Email,
            })
            .ToArrayAsync();
    }

    public async Task<bool> UpdateUserInfo(int userId, string name, string email)
    {
        var user = await context.Users.FirstAsync(u => u.Id == userId);
        user.Name = name;
        user.Email = email;
        var rowsEffected = await context.SaveChangesAsync();
        return rowsEffected > 0;
    }
}
