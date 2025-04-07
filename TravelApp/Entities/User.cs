﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TravelApp.Entities;

public class User
{
    [System.ComponentModel.DataAnnotations.Key]
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
    [IgnoreMember]
    public UserPreferences Preferences
    {
        get => string.IsNullOrEmpty(PreferencesJson) ? new UserPreferences() : MessagePackSerializer.Deserialize<UserPreferences>(Convert.FromBase64String(PreferencesJson));
        set => PreferencesJson = Convert.ToBase64String(MessagePackSerializer.Serialize(value));
    }

    public string PreferencesJson { get; set; } // Base64 encoded MessagePack data

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
