using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TravelApp.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    public string PhoneNumber { get; set; }
    public string Country { get; set; }
    public string State { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    [JsonIgnore]
    public UserPreferences Preferences
    {
        get => string.IsNullOrEmpty(PreferencesJson) ? new UserPreferences() : JsonSerializer.Deserialize<UserPreferences>(PreferencesJson);
        set => PreferencesJson = JsonSerializer.Serialize(value);
    }

    public string PreferencesJson { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
