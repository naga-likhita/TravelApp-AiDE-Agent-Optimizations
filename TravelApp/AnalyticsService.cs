﻿using TravelApp.Entities;
using System.Collections.Generic;
using System.Linq;

namespace TravelApp;

public class AnalyticsService
{
    public List<string> GetTop5Destinations(List<Booking> bookings)
    {
        // Step 1: Count occurrences using a dictionary
        var destinationCounts = new Dictionary<string, int>();

        foreach (var booking in bookings)
        {
            var destination = booking.Flight.Destination;
            if (destinationCounts.ContainsKey(destination))
                destinationCounts[destination]++;
            else
                destinationCounts[destination] = 1;
        }

        // Step 2: Sort and take top 5
        var top5 = destinationCounts
            .OrderByDescending(pair => pair.Value)
            .ThenBy(pair => pair.Key) // optional: for consistent ordering
            .Take(5)
            .Select(pair => pair.Key)
            .ToList();

        return top5;
    }
}
