using EmergencySimulator.AdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public class StaffRepository : Repository<Staff>, IStaffRepository
    {
        public StaffRepository(DatabaseContext context) : base(context) { }

        public IEnumerable<Staff> GetAllWithDetails()
        {
            return _context.Staff
                .Include(s => s.NotificationList)
                .Include(s => s.VGKMember)
                .ToList();
        }

        public async Task<IEnumerable<Staff>> GetAllWithDetailsAsync()
        {
            return await _context.Staff
                .Include(s => s.NotificationList)
                .Include(s => s.VGKMember)
                .ToListAsync();
        }

        public IEnumerable<Staff> SearchStaff(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllWithDetails();

            return _context.Staff
                .Include(s => s.NotificationList)
                .Where(s => s.Name.Contains(searchText) ||
                           s.Surname.Contains(searchText) ||
                           s.Position.Contains(searchText))
                .ToList();
        }

        public async Task<IEnumerable<Staff>> SearchStaffAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return await GetAllWithDetailsAsync();

            return await _context.Staff
                .Include(s => s.NotificationList)
                .Where(s => s.Name.Contains(searchText) ||
                           s.Surname.Contains(searchText) ||
                           s.Position.Contains(searchText))
                .ToListAsync();
        }

        public IEnumerable<Staff> GetActiveStaff()
        {
            return _context.Staff
                .Where(s => s.IsActive)
                .Include(s => s.NotificationList)
                .OrderBy(s => s.Surname)
                .ThenBy(s => s.Name)
                .ToList();
        }

        public IEnumerable<Staff> GetStaffByNotificationList(int listId)
        {
            return _context.Staff
                .Where(s => s.ListID == listId)
                .Include(s => s.NotificationList)
                .ToList();
        }
    }
}
