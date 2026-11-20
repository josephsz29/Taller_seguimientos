using System.ComponentModel.DataAnnotations;

namespace OnlineChronometer.Models
{
    // Un cronómetro. El servidor NO guarda "el tiempo que va": guarda cuándo arrancó y cuánto había acumulado,
    // y de ahí calcula el tiempo en cualquier momento. Así el cronómetro sigue corriendo aunque se cierre la página.
    public class Chronometer
    {
        // Identificador aleatorio e imposible de adivinar (no un número consecutivo)
        public Guid Id { get; set; }

        [Required]
        [StringLength(60)]
        public string Name { get; set; } = string.Empty;

        // Milisegundos acumulados en los tramos anteriores (antes de la última vez que se pausó)
        public long AccumulatedMilliseconds { get; set; }

        // Momento (en hora UTC) en que arrancó el tramo actual. Es null cuando el cronómetro está detenido o en pausa.
        public DateTime? RunningSince { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Un cronómetro tiene muchas vueltas
        public List<ChronometerLap> Laps { get; set; } = new();

        // Propiedad calculada (solo lectura): EF no la guarda en la base de datos
        public bool IsRunning => RunningSince.HasValue;

        // Tiempo total transcurrido en el momento indicado = lo acumulado + el tramo que está corriendo
        public long GetElapsedMilliseconds(DateTime nowUtc)
        {
            if (!RunningSince.HasValue)
            {
                return AccumulatedMilliseconds;
            }

            var currentSegment = (long)Math.Round((nowUtc - RunningSince.Value).TotalMilliseconds);
            return AccumulatedMilliseconds + Math.Max(0, currentSegment);
        }
    }
}
