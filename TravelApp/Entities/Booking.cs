using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TravelApp.Entities;

public class Booking
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    [JsonIgnore]
    public User User { get; set; }

    [Required]
    public int FlightId { get; set; }

    [JsonIgnore]
    [ForeignKey("FlightId")]
    public Flight Flight { get; set; }

    [Required]
    public DateOnly TravelDate { get; set; }  // Date the user wants to fly

    public DateOnly BookingDate { get; set; }

    [Required]
    public BookingStatus Status { get; set; }

    public enum BookingStatus
    {
        CONFIRMED,
        CANCELLED,
        PENDING,
        CHECKED_IN,
        BOARDED,
        COMPLETED,
        NO_SHOW
    }

}
