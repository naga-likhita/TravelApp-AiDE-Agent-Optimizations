namespace TravelApp.Entities;

public class UserPreferences
{
    public string PreferredAirlines { get; set; }

    public string SeatPreference { get; set; }

    public bool WantsMeal { get; set; }
    public bool IsVegetarian { get; set; }

    public string CabinClass { get; set; } // Economy, Business, First Class

    public bool WindowSeatPreference { get; set; }

    public bool WantsTelevision { get; set; }

    public bool SpecialAssistanceRequired { get; set; }
}