namespace OnlineChronometer.Models
{
    // Un cronómetro con sus datos principales, para la lista (no se guarda en la base de datos)
    public class ChronometerSummary
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsRunning { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public int LapCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
