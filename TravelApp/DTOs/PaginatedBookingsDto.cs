using System.Collections.Generic;
using TravelApp.Entities;

namespace TravelApp.DTOs
{
    public class PaginatedBookingsDto
    {
        public List<Booking> Bookings { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
