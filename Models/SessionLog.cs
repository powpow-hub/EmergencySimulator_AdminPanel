using System;

namespace EmergencySimulator.AdminPanel.Models
{
    public class SessionLog
    {
        // Составной PK: (ResultID, LogEntryNumber)
        public int ResultID { get; set; }
        public int LogEntryNumber { get; set; }
        public DateTime EventTime { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string EventDescription { get; set; } = string.Empty;
        public string? Executor { get; set; }
        public string? Note { get; set; }

        // Навигационное свойство
        public virtual TrainingResult TrainingResult { get; set; } = null!;
    }
}
