using System;

namespace EmergencySimulator.AdminPanel.Models
{
    /// <summary>
    /// Модель администратора системы
    /// </summary>
    public class Admin
    {
        public int AdminID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Role { get; set; } = "Admin";
    }
}
