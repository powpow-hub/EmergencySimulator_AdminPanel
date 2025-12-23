using System;

namespace EmergencySimulator.AdminPanel.Models
{
    public class EmergencyCall
    {
        public int CallID { get; set; }
        public int ResultID { get; set; }
        public DateTime CallTime { get; set; }
        public string IncidentLocation { get; set; } = string.Empty;
        public int PeopleInZone { get; set; }
        public string? VictimName { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPosition { get; set; } = string.Empty;
        public bool AllFieldsFilled { get; set; } = false;

        // Навигационные свойства
        public virtual TrainingResult TrainingResult { get; set; } = null!;
    }
}
