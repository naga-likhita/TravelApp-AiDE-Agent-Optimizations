using Microsoft.EntityFrameworkCore;
using TravelApp.DTOs;
using TravelApp.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace TravelApp;

public class TravelRepo(TravelDbContext context, IMemoryCache memoryCache)
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    public static TravelRepo Instance => new(TravelDbContext.Instance, new MemoryCache(new MemoryCacheOptions()));

    /// <summary>
    /// This API used too frequently to get list of flights
    /// </summary>
    /// <returns></returns>
    public async Task<List<Flight>> GetFlightsAsync(string departure, string destination)
    {
        var cacheKey = $"flights_{departure}_{destination}";
        if (_memoryCache.TryGetValue(cacheKey, out List<Flight> cachedFlights))
        {
            return cachedFlights;
        }

        var flights = await context
            .Flights
            .Where(f => f.Departure == departure && f.Destination == destination)
            .AsNoTracking()
            .ToListAsync();

        _memoryCache.Set(cacheKey, flights, TimeSpan.FromMinutes(10)); // Cache for 10 minutes

        return flights;
    }

    public async Task<decimal> GetFlightSpeedAsync(int flightId)
    {
        var cacheKey = $"flightspeed_{flightId}";
        if (_memoryCache.TryGetValue(cacheKey, out decimal cachedSpeed))
        {
            return cachedSpeed;
        }

        var speed = await context.Flights
            .Where(f => f.Id == flightId)
            .Select(f => f.Speed)
            .FirstAsync();

        _memoryCache.Set(cacheKey, speed, TimeSpan.FromMinutes(10)); // Cache for 10 minutes

        return speed;
    }

    /// <summary>
    /// Large results as there will be many bookings in system
    /// </summary>
    /// <returns></returns>
    public async Task<List<Booking>> GetBookings()
    {
        return await context.Bookings.AsNoTracking().ToListAsync();
    }

    public async Task<List<Booking>> GetBookingsForUser(string userPhoneNum)
    {
        return await context.Bookings.Where(b => b.User.PhoneNumber == userPhoneNum).AsNoTracking().ToListAsync();
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
            })
            .AsNoTracking()
            .FirstAsync();
    }

    public async Task<UserInfoDto[]> GetUsersByCountry(string country)
    {
        var cacheKey = $"usersbycountry_{country}";
        if (_memoryCache.TryGetValue(cacheKey, out UserInfoDto[] cachedUsers))
        {
            return cachedUsers;
        }

        var users = (await context.Users
            .Where(u => u.Country == country)
            .Select(u => new UserInfoDto
            {
                Name = u.Name,
                Email = u.Email,
            })
            .AsNoTracking()
            .ToListAsync()).ToArray();

        _memoryCache.Set(cacheKey, users, TimeSpan.FromHours(1)); // Cache for 1 hour

        return users;
    }

    public async Task<bool> UpdateUserInfo(int userId, string name, string email)
    {
        var user = await context.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();

        if (user != null)
        {
            user.Name = name;
            user.Email = email;
            var rowsEffected = await context.SaveChangesAsync();

            // Invalidate cache for user info
            _memoryCache.Remove($"usersbycountry_{user.Country}");

            return rowsEffected > 0;
        }

        return false;
    }
}
