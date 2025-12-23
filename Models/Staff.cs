namespace EmergencySimulator.AdminPanel.Models
{
    public class Staff
    {
        public int StaffID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Surname { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int? ListID { get; set; }
        public bool IsActive { get; set; } = true;

        // Навигационные свойства
        public virtual NotificationList? NotificationList { get; set; }
        public virtual VGKMember? VGKMember { get; set; }
    }
}
