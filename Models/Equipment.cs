using System.Collections.Generic;

namespace EmergencySimulator.AdminPanel.Models
{
    public class Equipment
    {
        public int EquipmentID { get; set; }
        public string EquipmentName { get; set; } = string.Empty;
        public string EquipmentType { get; set; } = string.Empty;
        public string? Location { get; set; }
        public bool IsAvailable { get; set; } = true;

        // Навигационные свойства
        public virtual ICollection<SessionAction> SessionActions { get; set; } = new List<SessionAction>();
    }
}
