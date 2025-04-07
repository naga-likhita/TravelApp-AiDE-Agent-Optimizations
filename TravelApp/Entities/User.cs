using MessagePack;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

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

    // Store raw MessagePack byte array as Base64 string
    [NotMapped]
    public UserPreferences Preferences
    {
        get => string.IsNullOrEmpty(PreferencesMsgPack)
            ? new UserPreferences()
            : MessagePackSerializer.Deserialize<UserPreferences>(Convert.FromBase64String(PreferencesMsgPack));

        set => PreferencesMsgPack = Convert.ToBase64String(MessagePackSerializer.Serialize(value));
    }

    public string PreferencesMsgPack { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
