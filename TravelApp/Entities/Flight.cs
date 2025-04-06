using System.ComponentModel.DataAnnotations;

namespace TravelApp.Entities;

public class Flight
{
    [Key]
    public int Id { get; set; }
    public string AirlineName { get; set; }

    [Required]
    public string FlightNumber { get; set; }

    [Required]
    public string Departure { get; set; }

    [Required]
    public string Destination { get; set; }

    public TimeSpan ScheduledDepartureTime { get; set; } // Daily time, e.g., 10:00 AM

    public TimeSpan ScheduledArrivalTime { get; set; }   // Daily time, e.g., 12:30 PM

    [Required]
    public decimal Price { get; set; }
    public decimal Speed { get; set; }
}
