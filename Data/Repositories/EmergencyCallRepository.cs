using EmergencySimulator.AdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public class EmergencyCallRepository : Repository<EmergencyCall>, IEmergencyCallRepository
    {
        public EmergencyCallRepository(DatabaseContext context) : base(context) { }

        public IEnumerable<EmergencyCall> GetByResultId(int resultId)
        {
            return _context.EmergencyCalls
                .Where(c => c.ResultID == resultId)
                .OrderBy(c => c.CallTime)
                .ToList();
        }

        public async Task<IEnumerable<EmergencyCall>> GetByResultIdAsync(int resultId)
        {
            return await _context.EmergencyCalls
                .Where(c => c.ResultID == resultId)
                .OrderBy(c => c.CallTime)
                .ToListAsync();
        }

        public IEnumerable<EmergencyCall> GetIncompleteCallsByResultId(int resultId)
        {
            return _context.EmergencyCalls
                .Where(c => c.ResultID == resultId && !c.AllFieldsFilled)
                .ToList();
        }
    }
}
