using TravelApp.Entities;
using static TravelApp.Entities.Booking;

namespace TravelApp.Mock;

public class Mocker
{
    public static void MockFlights(TravelDbContext context)
    {
        var random = new Random();

        string[] airlines = context.Users.Select(u => u.Preferences).ToList().Select(p => p.PreferredAirlines).Distinct().ToArray();
        string[] cities = new[] { "Mumbai", "Delhi", "Bangalore", "Hyderabad", "Chennai", "Kolkata", "Ahmedabad", "Pune", "Jaipur", "Goa" };

        List<Flight> flights = new();
        int flightCounter = 1;

        // Step 1: Generate all valid city pairs (A -> B, where A ≠ B)
        var routes = new List<(string From, string To)>();
        foreach (var from in cities)
        {
            foreach (var to in cities)
            {
                if (!from.Equals(to, StringComparison.OrdinalIgnoreCase))
                    routes.Add((from, to));
            }
        }

        // Step 2: Shuffle routes
        routes = routes.OrderBy(_ => random.Next()).ToList();

        // Step 3: Generate flights for each route (limit to 1000)
        int count = Math.Min(1000, routes.Count);
        for (int i = 0; i < count; i++)
        {
            var (departure, destination) = routes[i];
            var airline = airlines[random.Next(airlines.Length)];

            int depHour = random.Next(0, 23);
            int depMinute = random.Next(0, 60);
            var depTime = new TimeSpan(depHour, depMinute, 0);

            int durationHours = random.Next(1, 5); // Duration 1–4 hrs
            int durationMinutes = random.Next(0, 60);
            var arrTime = depTime.Add(new TimeSpan(durationHours, durationMinutes, 0));

            flights.Add(new Flight
            {
                AirlineName = airline,
                FlightNumber = $"{airline.Split(' ')[0].Substring(0, 2).ToUpper()}{flightCounter++:D3}",
                Departure = departure,
                Destination = destination,
                ScheduledDepartureTime = depTime,
                ScheduledArrivalTime = arrTime,
                Price = Math.Round((decimal)(random.NextDouble() * 5000 + 2000), 2),
                Speed = Math.Round((decimal)(random.NextDouble() * 200 + 600), 2)
            });
        }
        context.AddRange(flights);
        context.SaveChanges();
    }

    public static void MockBookings(TravelDbContext context)
    {
        var random = new Random();
        var statuses = Enum.GetValues<BookingStatus>();

        // Load users and flights (you can deserialize from existing JSONs)
        List<User> users = context.Users.ToList();     // Assume it returns List<User>
        List<Flight> flights = context.Flights.ToList(); // Assume it returns List<Flight>

        List<Booking> bookings = new();

        for (int i = 0; i < 10000; i++)
        {
            var user = users[random.Next(users.Count)];
            var flight = flights[random.Next(flights.Count)];

            // Random travel date within the next 60 days
            var travelDate = DateTime.Today.AddDays(random.Next(1, 60));

            // Booking date is before travel date
            var bookingDate = travelDate.AddDays(-random.Next(1, 30));

            var status = statuses[random.Next(statuses.Length)];

            var booking = new Booking
            {
                UserId = user.Id,
                FlightId = flight.Id,
                TravelDate = DateOnly.FromDateTime(travelDate),
                BookingDate = DateOnly.FromDateTime(bookingDate),
                Status = status
            };

            bookings.Add(booking);
        }

        context.AddRange(bookings);
        context.SaveChanges();
    }
}
