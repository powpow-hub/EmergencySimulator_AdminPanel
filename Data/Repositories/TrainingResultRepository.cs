using EmergencySimulator.AdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public class TrainingResultRepository : Repository<TrainingResult>, ITrainingResultRepository
    {
        public TrainingResultRepository(DatabaseContext context) : base(context) { }

        public IEnumerable<TrainingResult> GetAllWithDetails()
        {
            return _context.TrainingResults
                .Include(r => r.User)
                .Include(r => r.SessionActions)
                .Include(r => r.SessionLogs)
                .Include(r => r.EmergencyCalls)
                .OrderByDescending(r => r.SessionStart)
                .ToList();
        }

        public async Task<IEnumerable<TrainingResult>> GetAllWithDetailsAsync()
        {
            return await _context.TrainingResults
                .Include(r => r.User)
                .Include(r => r.SessionActions)
                .Include(r => r.SessionLogs)
                .Include(r => r.EmergencyCalls)
                .OrderByDescending(r => r.SessionStart)
                .ToListAsync();
        }

        public IEnumerable<TrainingResult> GetByUserId(int userId)
        {
            return _context.TrainingResults
                .Where(r => r.UserID == userId)
                .Include(r => r.SessionActions)
                .OrderByDescending(r => r.SessionStart)
                .ToList();
        }

        public IEnumerable<TrainingResult> FilterResults(string searchText, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.TrainingResults
                .Include(r => r.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(r => r.User.Surname.Contains(searchText) ||
                                        r.User.Name.Contains(searchText) ||
                                        r.Status.Contains(searchText));
            }

            if (startDate.HasValue)
            {
                query = query.Where(r => r.SessionStart >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.SessionStart <= endDate.Value);
            }

            return query.OrderByDescending(r => r.SessionStart).ToList();
        }

        public async Task<IEnumerable<TrainingResult>> FilterResultsAsync(string searchText, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.TrainingResults
                .Include(r => r.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(r => r.User.Surname.Contains(searchText) ||
                                        r.User.Name.Contains(searchText) ||
                                        r.Status.Contains(searchText));
            }

            if (startDate.HasValue)
            {
                query = query.Where(r => r.SessionStart >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.SessionStart <= endDate.Value);
            }

            return await query.OrderByDescending(r => r.SessionStart).ToListAsync();
        }
    }
}
