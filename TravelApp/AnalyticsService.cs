using System.Collections.Generic;
using System.Linq;
using TravelApp.Entities;

namespace TravelApp;

public class AnalyticsService
{
    public List<string> GetTop5Destinations(List<Booking> bookings)
    {
        var destinationCounts = new Dictionary<string, int>();

        foreach (var booking in bookings)
        {
            string destination = booking.Flight.Destination;
            if (destinationCounts.ContainsKey(destination))
            {
                destinationCounts[destination]++;
            }
            else
            {
                destinationCounts[destination] = 1;
            }
        }

        var topDestinations = destinationCounts
            .OrderByDescending(pair => pair.Value)
            .Take(5)
            .Select(pair => pair.Key)
            .ToList();

        return topDestinations;
    }
}
