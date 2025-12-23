using EmergencySimulator.AdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public class SessionLogRepository : Repository<SessionLog>, ISessionLogRepository
    {
        public SessionLogRepository(DatabaseContext context) : base(context) { }

        public IEnumerable<SessionLog> GetByResultId(int resultId)
        {
            return _context.SessionLogs
                .Where(l => l.ResultID == resultId)
                .OrderBy(l => l.EventTime)
                .ToList();
        }

        public async Task<IEnumerable<SessionLog>> GetByResultIdAsync(int resultId)
        {
            return await _context.SessionLogs
                .Where(l => l.ResultID == resultId)
                .OrderBy(l => l.EventTime)
                .ToListAsync();
        }

        public IEnumerable<SessionLog> GetByUserId(int userId)
        {
            return _context.SessionLogs
                .Where(l => l.UserID == userId)
                .OrderByDescending(l => l.EventTime)
                .ToList();
        }
    }
}
