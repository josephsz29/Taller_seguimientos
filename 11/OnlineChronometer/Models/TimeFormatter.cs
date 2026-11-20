namespace OnlineChronometer.Models
{
    public static class TimeFormatter
    {
        // Convierte milisegundos en texto: "05:23.47" (min:seg.centésimas) o "1:05:23.47" si pasa de una hora.
        // El JavaScript (chronometer.js) usa exactamente el mismo formato.
        public static string Format(long milliseconds)
        {
            var totalCentiseconds = milliseconds / 10;
            var centiseconds = totalCentiseconds % 100;
            var totalSeconds = totalCentiseconds / 100;
            var seconds = totalSeconds % 60;
            var totalMinutes = totalSeconds / 60;
            var minutes = totalMinutes % 60;
            var hours = totalMinutes / 60;

            var text = $"{minutes:D2}:{seconds:D2}.{centiseconds:D2}";

            return hours > 0 ? $"{hours}:{text}" : text;
        }
    }
}
