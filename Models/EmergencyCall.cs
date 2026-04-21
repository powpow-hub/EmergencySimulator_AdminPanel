using System;

namespace EmergencySimulator.AdminPanel.Models
{
    public class EmergencyCall
    {
        // Составной PK: (ResultID, CallTime)
        public int ResultID { get; set; }
        public DateTime CallTime { get; set; }
        public string IncidentLocation { get; set; } = string.Empty;
        public int PeopleInZone { get; set; }       // новое поле
        public string? VictimName { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPosition { get; set; } = string.Empty;
        // AllFieldsFilled убран — его нет в схеме БД

        // Навигационное свойство
        public virtual TrainingResult TrainingResult { get; set; } = null!;
    }
}
