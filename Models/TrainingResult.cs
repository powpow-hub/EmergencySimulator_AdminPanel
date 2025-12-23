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
        public int CriticalErrorsCount { get; set; } = 0;
        public int SignificantErrorsCount { get; set; } = 0;
        public int TimeViolationsCount { get; set; } = 0;
        public string Status { get; set; } = "В процессе";
        public string? PDFReportPath { get; set; }

        // Навигационные свойства
        public virtual User User { get; set; } = null!;
        public virtual ICollection<SessionAction> SessionActions { get; set; } = new List<SessionAction>();
        public virtual SessionLog? SessionLog { get; set; }
        public virtual ICollection<EmergencyCall> EmergencyCalls { get; set; } = new List<EmergencyCall>();
    }
}
