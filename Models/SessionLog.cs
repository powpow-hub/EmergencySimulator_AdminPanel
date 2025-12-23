using System;

namespace EmergencySimulator.AdminPanel.Models
{
    public class SessionLog
    {
        public int LogID { get; set; }
        public int ResultID { get; set; }
        public int UserID { get; set; }
        public DateTime EventTime { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string EventDescription { get; set; } = string.Empty;
        public string? Executor { get; set; }
        public string? Note { get; set; }

        // Навигационные свойства
        public virtual TrainingResult TrainingResult { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
