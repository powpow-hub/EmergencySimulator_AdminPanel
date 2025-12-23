namespace EmergencySimulator.AdminPanel.Models
{
    public class VGKMember
    {
        public int MemberID { get; set; }
        public int StaffID { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsCommander { get; set; } = false;
        public bool IsOnDuty { get; set; } = true;

        // Навигационные свойства
        public virtual Staff Staff { get; set; } = null!;
    }
}
