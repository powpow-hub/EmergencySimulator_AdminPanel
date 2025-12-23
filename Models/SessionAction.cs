using System;

namespace EmergencySimulator.AdminPanel.Models
{
    public class SessionAction
    {
        public int ActionID { get; set; }
        public int ResultID { get; set; }
        public int? PlaID { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public DateTime ActionTime { get; set; }
        public int TimeTaken { get; set; }
        public bool IsCompleted { get; set; } = false;
        public bool IsCorrect { get; set; } = false;
        public string? ErrorType { get; set; }
        public string? ErrorDescription { get; set; }
        public int? EquipmentID { get; set; }

        // Навигационные свойства
        public virtual TrainingResult TrainingResult { get; set; } = null!;
        public virtual PLAData? PLAData { get; set; }
        public virtual Equipment? Equipment { get; set; }
    }
}
