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
                .Where(a => a.ResultID == resultId)
                .Include(a => a.PLAData)
                .Include(a => a.Equipment)
                .OrderBy(a => a.ActionOrder)
                .ToList();
        }

        public async Task<IEnumerable<SessionAction>> GetByResultIdAsync(int resultId)
        {
            return await _context.SessionActions
                .Where(a => a.ResultID == resultId)
                .Include(a => a.PLAData)
                .Include(a => a.Equipment)
                .OrderBy(a => a.ActionOrder)
                .ToListAsync();
        }

        // Действия с любой ошибкой в рамках сессии
        public IEnumerable<SessionAction> GetErrorActions(int resultId)
        {
            return _context.SessionActions
                .Where(a => a.ResultID == resultId && !a.IsCorrect)
                .Include(a => a.PLAData)
                .OrderBy(a => a.ActionOrder)
                .ToList();
        }

        // Критические ошибки определяются по полю ErrorType
        public IEnumerable<SessionAction> GetCriticalErrorActions(int resultId)
        {
            return _context.SessionActions
                .Where(a => a.ResultID == resultId && a.ErrorType == "Критическая")
                .Include(a => a.PLAData)
                .OrderBy(a => a.ActionOrder)
                .ToList();
        }
    }
}
