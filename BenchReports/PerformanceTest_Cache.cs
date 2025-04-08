using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Caching.Memory;
using TravelApp;

namespace BenchReports
{
    public class PerformanceTest_Cache : PerformanceTest_NoCache
    {
        public override async Task Setup()
        {
            Console.WriteLine("In memory cache setting up...");
            TravelRepo._cache = new MemoryCache(new MemoryCacheOptions());
            await GetFlights();
            await GetBookings();
            await GetBookingsForUser();
            await GetUserInfoByEmail();
            await GetUsersByCountry();
            await SendNotification();
            await ReminderUsersAsync();
            Console.WriteLine("In memory cache setup completed");
        }
    }
}
