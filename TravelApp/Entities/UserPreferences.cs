using MessagePack;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = MessagePack.KeyAttribute;

namespace TravelApp.Entities;

[MessagePackObject]
public class UserPreferences
{
    [Key(0)] public string PreferredAirlines { get; set; }
    [Key(1)] public string SeatPreference { get; set; }
    [Key(2)] public bool WantsMeal { get; set; }
    [Key(3)] public bool IsVegetarian { get; set; }
    [Key(4)] public string CabinClass { get; set; }
    [Key(5)] public bool WindowSeatPreference { get; set; }
    [Key(6)] public bool WantsTelevision { get; set; }
    [Key(7)] public bool SpecialAssistanceRequired { get; set; }
}
