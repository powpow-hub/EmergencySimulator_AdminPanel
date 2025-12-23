using System.Collections.Generic;

namespace EmergencySimulator.AdminPanel.Models
{
    public class PLAData
    {
        public int PlaID { get; set; }
        public string PositionNumber { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public string ActionStep { get; set; } = string.Empty;
        public int StepOrder { get; set; }
        public int TimeNormative { get; set; }
        public bool IsCritical { get; set; } = false;
        public string? Description { get; set; }

        // Навигационные свойства
        public virtual ICollection<SessionAction> SessionActions { get; set; } = new List<SessionAction>();
    }
}
