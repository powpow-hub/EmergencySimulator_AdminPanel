using EmergencySimulator.AdminPanel.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public interface ITrainingResultRepository : IRepository<TrainingResult>
    {
        IEnumerable<TrainingResult> GetAllWithDetails();
        Task<IEnumerable<TrainingResult>> GetAllWithDetailsAsync();
        IEnumerable<TrainingResult> GetByUserId(int userId);
        IEnumerable<TrainingResult> FilterResults(string searchText, DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<TrainingResult>> FilterResultsAsync(string searchText, DateTime? startDate, DateTime? endDate);
    }
}
