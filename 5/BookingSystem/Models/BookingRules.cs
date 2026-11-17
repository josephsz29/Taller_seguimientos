namespace BookingSystem.Models
{
    public static class BookingRules
    {
        // Horario de atención (de lunes a viernes)
        public static readonly TimeSpan OpeningTime = TimeSpan.FromHours(8);
        public static readonly TimeSpan ClosingTime = TimeSpan.FromHours(17);

        // Minutos que un horario queda apartado mientras el usuario confirma su reserva
        public const int PendingHoldMinutes = 10;

        public static string OpeningLabel => OpeningTime.ToString(@"hh\:mm");
        public static string ClosingLabel => ClosingTime.ToString(@"hh\:mm");

        public static bool IsBusinessDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }
    }
}
