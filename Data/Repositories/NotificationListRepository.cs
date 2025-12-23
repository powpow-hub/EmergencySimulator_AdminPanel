using EmergencySimulator.AdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public class NotificationListRepository : Repository<NotificationList>, INotificationListRepository
    {
        public NotificationListRepository(DatabaseContext context) : base(context) { }

        public IEnumerable<NotificationList> GetAllWithMembers()
        {
            return _context.NotificationLists
                .Include(l => l.StaffMembers)
                .OrderBy(l => l.Priority)
                .ToList();
        }

        public async Task<IEnumerable<NotificationList>> GetAllWithMembersAsync()
        {
            return await _context.NotificationLists
                .Include(l => l.StaffMembers)
                .OrderBy(l => l.Priority)
                .ToListAsync();
        }

        public NotificationList GetByIdWithMembers(int id)
        {
            return _context.NotificationLists
                .Include(l => l.StaffMembers)
                .FirstOrDefault(l => l.ListID == id);
        }

        public async Task<NotificationList> GetByIdWithMembersAsync(int id)
        {
            return await _context.NotificationLists
                .Include(l => l.StaffMembers)
                .FirstOrDefaultAsync(l => l.ListID == id);
        }

        public IEnumerable<NotificationList> GetByPriority(int priority)
        {
            return _context.NotificationLists
                .Where(l => l.Priority == priority)
                .ToList();
        }
    }
}
