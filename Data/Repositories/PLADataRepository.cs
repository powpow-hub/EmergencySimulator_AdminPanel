using EmergencySimulator.AdminPanel.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public class PLADataRepository : Repository<PLAData>, IPLADataRepository
    {
        public PLADataRepository(DatabaseContext context) : base(context) { }

        public IEnumerable<PLAData> GetByPosition(string positionNumber)
        {
            return _context.PLA_Data
                .Where(p => p.PositionNumber == positionNumber)
                .OrderBy(p => p.StepOrder)
                .ToList();
        }

        public async Task<IEnumerable<PLAData>> GetByPositionAsync(string positionNumber)
        {
            return await _context.PLA_Data
                .Where(p => p.PositionNumber == positionNumber)
                .OrderBy(p => p.StepOrder)
                .ToListAsync();
        }

        public IEnumerable<PLAData> GetCriticalActions()
        {
            return _context.PLA_Data
                .Where(p => p.IsCritical)
                .OrderBy(p => p.PositionNumber)
                .ThenBy(p => p.StepOrder)
                .ToList();
        }

        public IEnumerable<PLAData> SearchPLA(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAll();

            return _context.PLA_Data
                .Where(p => p.PositionNumber.Contains(searchText) ||
                           p.PositionName.Contains(searchText) ||
                           p.ActionStep.Contains(searchText) ||
                           (p.Description != null && p.Description.Contains(searchText)))
                .OrderBy(p => p.PositionNumber)
                .ThenBy(p => p.StepOrder)
                .ToList();
        }
    }
}
