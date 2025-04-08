using Microsoft.EntityFrameworkCore;

namespace TravelApp;

public class AnalyticsService
{
    public List<string> GetTop5Destinations()
    {
        var bookings = TravelDbContext.Instance.Bookings.Include(b => b.Flight).ToList();
        var destinations = new List<string>();

        foreach (var booking in bookings)
        {
            destinations.Add(booking.Flight.Destination);
        }

        var topDestinations = new List<string>();

        foreach (var dest in destinations)
        {
            int count = 0;

            foreach (var d in destinations)
            {
                if (d == dest)
                    count++;
            }

            if (!topDestinations.Contains(dest))
            {
                int insertIndex = 0;
                while (insertIndex < topDestinations.Count &&
                       CountOccurrences(topDestinations[insertIndex], destinations) > count)
                {
                    insertIndex++;
                }

                topDestinations.Insert(insertIndex, dest);
                if (topDestinations.Count > 5)
                {
                    topDestinations.RemoveAt(topDestinations.Count - 1);
                }
            }
        }

        return topDestinations;
    }

    private int CountOccurrences(string value, List<string> list)
    {
        int count = 0;
        foreach (var item in list)
        {
            if (item == value)
                count++;
        }
        return count;
    }
}
