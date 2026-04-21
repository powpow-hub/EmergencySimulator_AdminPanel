using System;
using System.Collections.Generic;

namespace EmergencySimulator.AdminPanel.Models
{
    public class TrainingResult
    {
        public int ResultID { get; set; }
        public int UserID { get; set; }
        public DateTime SessionStart { get; set; }
        public DateTime? SessionEnd { get; set; }
        public int TotalScore { get; set; } = 0;
        public double CriticalErrorsCount { get; set; } = 0;    // REAL → double
        public string SignificantErrorsCount { get; set; } = "0"; // TEXT → string
        public int TimeViolationsCount { get; set; } = 0;
        public string Status { get; set; } = "В процессе";
        public string? PDFReportPath { get; set; }

        // Навигационные свойства
        public virtual User User { get; set; } = null!;
        public virtual ICollection<SessionAction> SessionActions { get; set; } = new List<SessionAction>();
        public virtual ICollection<SessionLog> SessionLogs { get; set; } = new List<SessionLog>(); // коллекция, не одиночный
        public virtual ICollection<EmergencyCall> EmergencyCalls { get; set; } = new List<EmergencyCall>();
    }
}
