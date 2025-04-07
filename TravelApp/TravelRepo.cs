using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TravelApp.DTOs;
using TravelApp.Entities;

namespace TravelApp;

public class TravelRepo(TravelDbContext context, IMemoryCache cache)
{
    private readonly IMemoryCache _cache = cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);
    public static TravelRepo Instance => new(TravelDbContext.Instance, new MemoryCache(new MemoryCacheOptions()));

    /// <summary>
    /// This API used too frequently to get list of flights
    /// </summary>
    /// <returns></returns>
    public async Task<List<Flight>> GetFlightsAsync(string departure, string destination)
    {
        var cacheKey = $"Flights_{departure}_{destination}";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
            return await context.Flights
                .Where(f => f.Departure == departure && f.Destination == destination)
                .AsNoTracking()
                .ToListAsync();
        });
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
    public async Task<(List<Booking> Results, int TotalCount, int TotalPages)> GetBookings(int pageNumber = 1, int pageSize = 50)
    {
        var query = context.Bookings.AsNoTracking().OrderBy(b => b.Id);
        var totalCount = await query.CountAsync();
        var results = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (results, totalCount, (int)Math.Ceiling(totalCount / (double)pageSize));
    }

    public async Task<List<Booking>> GetBookingsForUser(string userPhoneNum)
    {
        return await context.Bookings
            .Where(b => b.User.PhoneNumber == userPhoneNum)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// User visits profile page multiple times
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<UserInfoDto> GetUserInfoByEmail(string email)
    {
        var cacheKey = $"User_Email_{email}";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
            return await context.Users.Where(u => u.Email == email)
                .Select(user => new UserInfoDto
                {
                    Name = user.Name,
                    Email = user.Email,
                }).FirstAsync();
        });
    }

    public async Task<UserInfoDto[]> GetUsersByCountry(string country)
    {
        var cacheKey = $"Users_Country_{country}";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
            return await context.Users
                .Where(u => u.Country == country)
                .Select(u => new UserInfoDto
                {
                    Name = u.Name,
                    Email = u.Email,
                })
                .ToArrayAsync();
        });
    }

    public async Task<bool> UpdateUserInfo(int userId, string name, string email)
    {
        var user = await context.Users.FirstAsync(u => u.Id == userId);
        user.Name = name;
        user.Email = email;
        var rowsEffected = await context.SaveChangesAsync();

        if (rowsEffected > 0)
        {
            // Invalidate all cached data for this user
            var cachedUser = await context.Users.FindAsync(userId);
            if (cachedUser != null)
            {
                _cache.Remove($"User_Email_{cachedUser.Email}");
                _cache.Remove($"Bookings_{cachedUser.PhoneNumber}");
                _cache.Remove($"Users_Country_{cachedUser.Country}");
            }
        }
        return rowsEffected > 0;
    }
}
