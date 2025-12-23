using EmergencySimulator.AdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public interface IAdminRepository : IRepository<Admin>
    {
        Admin GetByUsername(string username);
        bool ValidateCredentials(string username, string password);
        void UpdateLastLogin(int adminId);
    }

    public class AdminRepository : Repository<Admin>, IAdminRepository
    {
        public AdminRepository(DatabaseContext context) : base(context)
        {
        }

        public Admin GetByUsername(string username)
        {
            return _context.Admins.FirstOrDefault(a => a.Username == username && a.IsActive);
        }

        public bool ValidateCredentials(string username, string password)
        {
            var admin = GetByUsername(username);
            if (admin == null) return false;

            // Проверяем хеш пароля
            string hashedPassword = HashPassword(password, admin.Salt);
            return admin.PasswordHash == hashedPassword;
        }

        public void UpdateLastLogin(int adminId)
        {
            var admin = GetById(adminId);
            if (admin != null)
            {
                admin.LastLogin = DateTime.Now;
                Update(admin);
            }
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
