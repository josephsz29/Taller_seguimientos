namespace OnlineChronometer.Models
{
    // Datos de la página del cronómetro
    public class ChronometerDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Estado inicial en formato JSON; el JavaScript lo lee desde un atributo data-*
        public string StateJson { get; set; } = string.Empty;
    }
}
