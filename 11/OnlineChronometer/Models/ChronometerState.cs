namespace OnlineChronometer.Models
{
    // Estado completo de un cronómetro tal como se envía al navegador (JSON)
    public class ChronometerState
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsRunning { get; set; }

        // Tiempo transcurrido calculado por el servidor en el momento de la respuesta
        public long ElapsedMilliseconds { get; set; }

        public List<LapState> Laps { get; set; } = new();

        // Números de la vuelta más rápida y la más lenta (solo si hay 2 o más vueltas distintas)
        public int? BestLapNumber { get; set; }
        public int? WorstLapNumber { get; set; }
    }
}
