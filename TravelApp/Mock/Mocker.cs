using System.Text.Json;
using TravelApp.Entities;
using static TravelApp.Entities.Booking;

namespace TravelApp.Mock;

public static class Mocker
{
    static string[] airlines = {
    "Air India",
    "IndiGo",
    "SpiceJet",
    "GoAir",
    "Vistara",
    "Qantas",
    "Singapore Airlines",
    "Emirates",
    "Cathay Pacific",
    "American Airlines",
    "Delta Airlines",
    "British Airways",
    "Lufthansa",
    "Air France"
};
    static Random random = new Random();
    public static void MockFlights(this TravelDbContext context, int count)
    {
        string[] cities = new[] {
        "Mumbai", "Delhi", "Bangalore", "Hyderabad", "Chennai", "Kolkata", "Ahmedabad", "Pune",
        "Jaipur", "Goa", "New York", "London", "Paris", "Sydney", "Singapore", "Dubai", "Tokyo",
        "Los Angeles", "Toronto", "Berlin", "Hong Kong", "Bangkok", "Amsterdam", "Rome", "Barcelona",
        "Seoul", "Beijing", "Istanbul", "Cairo", "Cape Town", "Moscow", "Athens", "Rio de Janeiro",
        "Mexico City", "Montreal", "Lagos", "Jakarta", "Sao Paulo", "Lima", "Buenos Aires", "Kuala Lumpur",
        "Lagos", "Manila", "Vancouver", "Zurich", "Vienna", "Oslo", "Stockholm", "Madrid"
        };


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
        int counter = 0;
        var flights = new List<Flight>();
        // Step 3: Generate flights for each route (limit to count)
        do
        {
            for (int i = 0; i < routes.Count; i++)
            {
                var (departure, destination) = routes[i];
                var airline = airlines[random.Next(airlines.Length)];

                // Departure Time: Ensure minutes are 00 or 05
                int depHour = random.Next(0, 23);
                int depMinute = random.Next(0, 12) * 5; // Ensure 00, 05, 10, 15, etc.
                var depTime = new TimeSpan(depHour, depMinute, 0);

                // Flight Duration: Ensure duration minutes are 00 or 05
                int durationHours = random.Next(1, 5); // Duration 1–4 hours
                int durationMinutes = random.Next(0, 12) * 5; // Ensure 00, 05, 10, 15, etc.
                var arrTime = depTime.Add(new TimeSpan(durationHours, durationMinutes, 0));

                // Ensure arrival time has minutes 00 or 05
                int adjustedMinutes = arrTime.Minutes % 5 == 0 ? arrTime.Minutes : (arrTime.Minutes + 5) % 60;
                var adjustedArrTime = new TimeSpan(arrTime.Hours, adjustedMinutes, 0);

                // Speed: Ensure speed is a multiple of 5 (e.g., 600, 605, 610, etc.)
                decimal speed = Math.Round((decimal)(random.Next(120, 180) * 5), 2);

                // Price: Ensure price is in increments of 100 (e.g., 2000, 2100, 2200, etc.)
                decimal price = Math.Round((decimal)(random.Next(200, 1000) * 5), 2);

                counter++;
                // Create flight
                flights.Add(new Flight
                {
                    AirlineName = airline,
                    FlightNumber = $"{airline.Split(' ')[0].Substring(0, 2).ToUpper()}{counter:D3}",
                    Departure = departure,
                    Destination = destination,
                    ScheduledDepartureTime = depTime,
                    ScheduledArrivalTime = adjustedArrTime,
                    Price = price,
                    Speed = speed
                });
            }
        }
        while (counter < count);
        context.AddRange(flights.Take(count));
        context.SaveChanges();
        Console.WriteLine("Mocked flights");
    }

    public static void MockBookings(this TravelDbContext context, int count)
    {
        var statuses = Enum.GetValues<BookingStatus>();

        var usersCount = context.Users.Count();
        var flightsCount = context.Flights.Count();

        List<Booking> bookings = new();

        for (int i = 0; i < count; i++)
        {
            // Random travel date within the next 180 days
            var travelDate = DateTime.Today.AddDays(random.Next(1, 180));

            // Booking date is before travel date
            var bookingDate = travelDate.AddDays(-random.Next(1, 60));

            var status = statuses[random.Next(statuses.Length)];

            var booking = new Booking
            {
                UserId = random.Next(1,usersCount),
                FlightId = random.Next(1,flightsCount),
                TravelDate = DateOnly.FromDateTime(travelDate),
                BookingDate = DateOnly.FromDateTime(bookingDate),
                Status = status
            };

            bookings.Add(booking);
        }

        context.AddRange(bookings);
        context.SaveChanges();
    }

    public static void MockUsers(this TravelDbContext travelDbContext, int numberOfUsers)
    {
        Dictionary<string, List<string>> countryStates = new Dictionary<string, List<string>>()
    {
        { "India", new List<string>
            {
                "Andhra Pradesh", "Arunachal Pradesh", "Assam", "Bihar", "Chhattisgarh", "Goa", "Gujarat", "Haryana",
                "Himachal Pradesh", "Jharkhand", "Karnataka", "Kerala", "Madhya Pradesh", "Maharashtra", "Manipur",
                "Meghalaya", "Mizoram", "Nagaland", "Odisha", "Punjab", "Rajasthan", "Sikkim", "Tamil Nadu", "Telangana",
                "Uttar Pradesh", "Uttarakhand", "West Bengal", "Delhi"
            }
        },
        { "United States", new List<string>
            {
                "California", "Texas", "New York", "Florida", "Illinois", "Pennsylvania", "Ohio", "Georgia", "North Carolina",
                "Michigan", "New Jersey", "Virginia", "Washington", "Arizona", "Massachusetts", "Tennessee", "Indiana",
                "Missouri", "Maryland", "Wisconsin"
            }
        },
        { "Australia", new List<string>
            {
                "New South Wales", "Queensland", "South Australia", "Tasmania", "Victoria", "Western Australia",
                "Australian Capital Territory", "Northern Territory"
            }
        },
        { "Singapore", new List<string>
            {
                "Central Singapore", "North East", "North West", "South East", "South West"
            }
        }
    };

        var countries = countryStates.Keys.ToList();
        countries.AddRange(["India", "India", "India"]);

        for (int i = 1; i <= numberOfUsers; i++)
        {
            var country = countries[random.Next(countryStates.Keys.Count - 1)];
            var user = new User
            {
                Id = i,
                Name = GenerateName(country),
                PhoneNumber = GeneratePhoneNumber(country),
                Country = country,
                State = countryStates[country][random.Next(countryStates[country].Count - 1)],
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                PreferencesJson = GenerateUserPreferences()
            };
            user.Email = GenerateEmail(user.Name, i);

            travelDbContext.Add(user);
        }
        travelDbContext.SaveChanges();
    }
    static string GenerateEmail(string userName, int id)
    {
        // Ensure the ID is a 2-digit number
        int twoDigitId = id % 90 + 10; // This will ensure the ID is between 10 and 99

        // List of valid email domains (Indian and some common domains)
        string[] domains = {
        "gmail.com", "yahoo.co.in", "outlook.com", "rediffmail.com", "indiatimes.com", "airtelmail.com"
    };

        // Choose a random domain from the list
        var random = new Random();
        string domain = domains[random.Next(domains.Length)];

        // Generate email address by appending id to username
        string email = $"{userName.ToLower()}.{twoDigitId}@{domain}";

        return email;
    }

    static string GenerateName(string country)
    {
        if (country == "India")
        {
            string[] firstNames = {
                "Aarav", "Vivaan", "Aditya", "Vihaan", "Arjun", "Sai", "Ishaan", "Ayaan", "Rohan", "Krishna",
                "Ananya", "Kavya", "Priya", "Isha", "Saanvi", "Aditi", "Riya", "Nisha", "Neha", "Madhav",
                "Siddharth", "Shaurya", "Aryan", "Tanvi", "Simran", "Anjali", "Maya", "Shivansh", "Yash", "Manav",
                "Meera", "Aarohi", "Sneha", "Pooja", "Swara", "Kiran", "Nikita", "Shubham", "Suryansh", "Tanishq",
                "Vishal", "Amit", "Shreya", "Ravina", "Lakshmi", "Tejas", "Ankita", "Raghav", "Tanmay", "Ritika"
            };

            string[] lastNames = {
                "Sharma", "Patel", "Singh", "Gupta", "Reddy", "Nair", "Kumar", "Yadav", "Desai", "Bhat",
                "Jadhav", "Mehta", "Chaudhary", "Verma", "Rathore", "Joshi", "Iyer", "Dixit", "Kohli", "Chowdhury",
                "Saxena", "Rajput", "Kapoor", "Shukla", "Chakraborty", "Ghosh", "Pandey", "Bansal", "Puri", "Bedi",
                "Malhotra", "Jain", "Sinha", "Mishra", "Tiwari", "Chandra", "Pillai", "Sarin", "Gupta", "Garg",
                "Bhattacharya", "Soni", "Chawla", "Rao", "Vyas"
            };
            return $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}";
        }
        else if (country == "United States")
        {
            string[] firstNames = { "James", "John", "Robert", "Michael", "David" };
            string[] lastNames = { "Smith", "Johnson", "Williams", "Jones", "Brown" };
            return $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}";
        }
        else if (country == "Australia")
        {
            string[] firstNames = { "Oliver", "Jack", "William", "Noah", "Leo" };
            string[] lastNames = { "Smith", "Jones", "Taylor", "Brown", "Wilson" };
            return $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}";
        }
        else if (country == "Singapore")
        {
            string[] firstNames = { "Wei", "Jie", "Jing", "Li", "Yun" };
            string[] lastNames = { "Tan", "Lim", "Ng", "Lee", "Wong" };
            return $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}";
        }
        return "Unknown Name";
    }

    static string GeneratePhoneNumber(string country)
    {
        if (country == "India")
            return $"+91-{random.Next(7, 10)}{random.Next(100000000, 999999999)}";
        else if (country == "United States")
            return $"+1-{random.Next(100, 999)}-{random.Next(100, 999)}-{random.Next(1000, 9999)}";
        else if (country == "Australia")
            return $"+61-{random.Next(100, 999)}-{random.Next(1000, 9999)}";
        else if (country == "Singapore")
            return $"+65-{random.Next(1000, 9999)}-{random.Next(1000, 9999)}";
        return "Unknown Number";
    }

    static string GenerateUserPreferences()
    {
        var preferences = new UserPreferences
        {
            PreferredAirlines = airlines[random.Next(airlines.Length)],
            SeatPreference = random.Next(0, 2) == 0 ? "Aisle" : "Window",
            WantsMeal = random.Next(0, 2) == 0,
            IsVegetarian = random.Next(0, 2) == 0,
            CabinClass = random.Next(0, 3) switch
            {
                0 => "Economy",
                1 => "Business",
                _ => "First Class"
            },
            WindowSeatPreference = random.Next(0, 2) == 0,
            WantsTelevision = random.Next(0, 2) == 0,
            SpecialAssistanceRequired = random.Next(0, 2) == 0
        };

        return JsonSerializer.Serialize(preferences);
    }
}
