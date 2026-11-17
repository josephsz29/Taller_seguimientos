namespace EventCalendar.Models
{
    public class ReminderOption
    {
        public int? Minutes { get; set; }
        public string Label { get; set; } = string.Empty;
    }

    // Opciones de recordatorio que se ofrecen en el formulario
    public static class ReminderOptions
    {
        public static readonly IReadOnlyList<ReminderOption> All = new List<ReminderOption>
        {
            new ReminderOption { Minutes = null, Label = "Sin recordatorio" },
            new ReminderOption { Minutes = 0, Label = "Al comenzar" },
            new ReminderOption { Minutes = 5, Label = "5 minutos antes" },
            new ReminderOption { Minutes = 15, Label = "15 minutos antes" },
            new ReminderOption { Minutes = 30, Label = "30 minutos antes" },
            new ReminderOption { Minutes = 60, Label = "1 hora antes" },
            new ReminderOption { Minutes = 1440, Label = "1 día antes" }
        };

        public static string GetLabel(int? minutes)
        {
            var option = All.FirstOrDefault(o => o.Minutes == minutes);
            return option?.Label ?? minutes + " minutos antes";
        }
    }
}
