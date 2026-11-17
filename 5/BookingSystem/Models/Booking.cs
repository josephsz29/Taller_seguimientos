using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }

        // Llave foránea y propiedad de navegación hacia el servicio reservado
        public int ServiceId { get; set; }
        public BookableService Service { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string CustomerEmail { get; set; } = string.Empty;

        [StringLength(30)]
        public string? CustomerPhone { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Código aleatorio que el usuario usa para consultar su reserva
        // (así no se exponen los Id consecutivos en la URL)
        [Required]
        [StringLength(10)]
        public string Code { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
