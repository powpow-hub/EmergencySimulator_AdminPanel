using System;
using System.Collections.Generic;

namespace EmergencySimulator.AdminPanel.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Surname { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
        public string? Salt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }

        // Навигационные свойства
        public virtual ICollection<TrainingResult> TrainingResults { get; set; } = new List<TrainingResult>();
        public virtual ICollection<SessionLog> SessionLogs { get; set; } = new List<SessionLog>();
    }
}
