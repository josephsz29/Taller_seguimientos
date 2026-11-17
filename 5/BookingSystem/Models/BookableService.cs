using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Models
{
    // Servicio que se puede reservar (por ejemplo, una consulta)
    public class BookableService
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Description { get; set; }

        public int DurationMinutes { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        // Un servicio tiene muchas reservas
        public List<Booking> Bookings { get; set; } = new();
    }
}
