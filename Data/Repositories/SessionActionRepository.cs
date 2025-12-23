using EmergencySimulator.AdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public class SessionActionRepository : Repository<SessionAction>, ISessionActionRepository
    {
        public SessionActionRepository(DatabaseContext context) : base(context) { }

        public IEnumerable<SessionAction> GetByResultId(int resultId)
        {
            return _context.SessionActions
                .Include(a => a.PLAData)
                .Include(a => a.Equipment)
                .Where(a => a.ResultID == resultId)
                .OrderBy(a => a.ActionTime)
                .ToList();
        }

        public async Task<IEnumerable<SessionAction>> GetByResultIdAsync(int resultId)
        {
            return await _context.SessionActions
                .Include(a => a.PLAData)
                .Include(a => a.Equipment)
                .Where(a => a.ResultID == resultId)
                .OrderBy(a => a.ActionTime)
                .ToListAsync();
        }

        public IEnumerable<SessionAction> GetErrorActions(int resultId)
        {
            return _context.SessionActions
                .Where(a => a.ResultID == resultId && !a.IsCorrect)
                .OrderBy(a => a.ActionTime)
                .ToList();
        }

        public IEnumerable<SessionAction> GetCriticalErrorActions()
        {
            return _context.SessionActions
                .Where(a => a.ErrorType == "Критическая")
                .Include(a => a.TrainingResult)
                    .ThenInclude(r => r.User)
                .ToList();
        }
    }
}
