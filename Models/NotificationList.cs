using System.Collections.Generic;

namespace EmergencySimulator.AdminPanel.Models
{
    public class NotificationList
    {
        public int ListID { get; set; }
        public string ListName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Priority { get; set; } = 1;

        // Навигационные свойства
        public virtual ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
    }
}
