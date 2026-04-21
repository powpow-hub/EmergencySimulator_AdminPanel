using EmergencySimulator.AdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DatabaseContext context) : base(context) { }

        public User GetByIdWithDetails(int id)
        {
            return _context.Users
                .Include(u => u.TrainingResults)
                // SessionLogs убран — UserID удалён из таблицы SessionLogs
                .FirstOrDefault(u => u.UserID == id);
        }

        public async Task<User> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Users
                .Include(u => u.TrainingResults)
                .FirstOrDefaultAsync(u => u.UserID == id);
        }

        public IEnumerable<User> GetAllWithDetails()
        {
            return _context.Users
                .Include(u => u.TrainingResults)
                .ToList();
        }

        public async Task<IEnumerable<User>> GetAllWithDetailsAsync()
        {
            return await _context.Users
                .Include(u => u.TrainingResults)
                .ToListAsync();
        }

        public IEnumerable<User> SearchUsers(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllWithDetails();

            return _context.Users
                .Include(u => u.TrainingResults)
                .Where(u => u.Name.Contains(searchText) ||
                            u.Surname.Contains(searchText) ||
                            (u.MiddleName != null && u.MiddleName.Contains(searchText)) ||
                            u.Position.Contains(searchText))
                .ToList();
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return await GetAllWithDetailsAsync();

            return await _context.Users
                .Include(u => u.TrainingResults)
                .Where(u => u.Name.Contains(searchText) ||
                            u.Surname.Contains(searchText) ||
                            (u.MiddleName != null && u.MiddleName.Contains(searchText)) ||
                            u.Position.Contains(searchText))
                .ToListAsync();
        }

        public User GetByUsername(string surname, string name)
        {
            return _context.Users
                .FirstOrDefault(u => u.Surname == surname && u.Name == name);
        }
    }
}