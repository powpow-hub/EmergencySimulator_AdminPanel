using System;

namespace EmergencySimulator.AdminPanel.Models
{
    public class SessionAction
    {
        // Составной PK: (ResultID, ActionOrder)
        public int ResultID { get; set; }
        public int ActionOrder { get; set; }
        public int PlaID { get; set; }          // NOT NULL в новой схеме
        public DateTime ActionTime { get; set; }
        public int TimeTaken { get; set; }
        public bool IsCompleted { get; set; } = false;
        public bool IsCorrect { get; set; } = false;
        public string? ErrorType { get; set; }
        public string? ErrorDescription { get; set; }
        public int? EquipmentID { get; set; }

        // Навигационные свойства
        public virtual TrainingResult TrainingResult { get; set; } = null!;
        public virtual PLAData PLAData { get; set; } = null!;   // NOT NULL
        public virtual Equipment? Equipment { get; set; }
    }
}
