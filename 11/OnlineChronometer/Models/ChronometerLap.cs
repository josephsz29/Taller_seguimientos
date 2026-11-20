namespace OnlineChronometer.Models
{
    // Una vuelta (tiempo parcial) registrada en un cronómetro
    public class ChronometerLap
    {
        public int Id { get; set; }

        public Guid ChronometerId { get; set; }
        public Chronometer Chronometer { get; set; } = null!;

        // Número de la vuelta: 1, 2, 3...
        public int Number { get; set; }

        // Tiempo total del cronómetro cuando se marcó la vuelta
        public long TotalMilliseconds { get; set; }

        // Tiempo de esta vuelta = tiempo total menos el tiempo total de la vuelta anterior
        public long LapMilliseconds { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
