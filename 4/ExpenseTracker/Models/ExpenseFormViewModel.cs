using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    // Datos del formulario para registrar un gasto
    public class ExpenseFormViewModel
    {
        [Required(ErrorMessage = "Escribe una descripción")]
        [StringLength(200, ErrorMessage = "La descripción no puede superar 200 caracteres")]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = string.Empty;

        // Nullable para que [Required] detecte el campo vacío en lugar de tomarlo como 0
        [Required(ErrorMessage = "Ingresa el monto")]
        [Range(0.01, 1000000000, ErrorMessage = "El monto debe ser mayor que cero")]
        [Display(Name = "Monto")]
        public decimal? Amount { get; set; }

        [Required(ErrorMessage = "Selecciona una fecha")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría")]
        [Display(Name = "Categoría")]
        public int CategoryId { get; set; }
    }
}
