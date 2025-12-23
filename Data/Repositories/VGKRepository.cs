using EmergencySimulator.AdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public class VGKRepository : Repository<VGKMember>, IVGKRepository
    {
        public VGKRepository(DatabaseContext context) : base(context) { }

        public IEnumerable<VGKMember> GetAllWithStaff()
        {
            return _context.VGKMembers
                .Include(m => m.Staff)
                    .ThenInclude(s => s.NotificationList)
                .OrderByDescending(m => m.IsCommander)
                .ThenBy(m => m.Staff.Surname)
                .ToList();
        }

        public async Task<IEnumerable<VGKMember>> GetAllWithStaffAsync()
        {
            return await _context.VGKMembers
                .Include(m => m.Staff)
                    .ThenInclude(s => s.NotificationList)
                .OrderByDescending(m => m.IsCommander)
                .ThenBy(m => m.Staff.Surname)
                .ToListAsync();
        }

        public VGKMember GetCommander()
        {
            return _context.VGKMembers
                .Include(m => m.Staff)
                .FirstOrDefault(m => m.IsCommander);
        }

        public async Task<VGKMember> GetCommanderAsync()
        {
            return await _context.VGKMembers
                .Include(m => m.Staff)
                .FirstOrDefaultAsync(m => m.IsCommander);
        }

        public IEnumerable<VGKMember> GetOnDutyMembers()
        {
            return _context.VGKMembers
                .Include(m => m.Staff)
                .Where(m => m.IsOnDuty)
                .ToList();
        }

        public IEnumerable<VGKMember> FilterMembers(string searchText, string role, bool onDutyOnly)
        {
            var query = _context.VGKMembers
                .Include(m => m.Staff)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(m => m.Staff.Surname.Contains(searchText) ||
                                        m.Staff.Name.Contains(searchText) ||
                                        m.Role.Contains(searchText));
            }

            if (!string.IsNullOrWhiteSpace(role) && role != "Все")
            {
                query = query.Where(m => m.Role == role);
            }

            if (onDutyOnly)
            {
                query = query.Where(m => m.IsOnDuty);
            }

            return query
                .OrderByDescending(m => m.IsCommander)
                .ThenBy(m => m.Staff.Surname)
                .ToList();
        }
    }
}
