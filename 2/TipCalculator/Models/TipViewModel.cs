using System.ComponentModel.DataAnnotations;

namespace TipCalculator.Models
{
    public class TipViewModel
    {
        // Nullable para que [Required] detecte el campo vacío en lugar de tomarlo como 0
        [Required(ErrorMessage = "Ingresa el monto de la cuenta")]
        [Range(0.01, 1000000000, ErrorMessage = "El monto debe ser mayor que cero")]
        [Display(Name = "Monto total de la cuenta")]
        public decimal? BillAmount { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        [Display(Name = "Porcentaje de propina")]
        public decimal TipPercentage { get; set; } = 10;

        // Es null hasta que se calcula la propina
        public TipResult? Result { get; set; }

        // Últimos cálculos guardados en la base de datos
        public List<TipRecord> History { get; set; } = new();
    }
}
