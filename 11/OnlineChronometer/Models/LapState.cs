namespace OnlineChronometer.Models
{
    // Una vuelta tal como se envía al navegador (JSON)
    public class LapState
    {
        public int Number { get; set; }
        public long LapMilliseconds { get; set; }
        public long TotalMilliseconds { get; set; }
    }
}
