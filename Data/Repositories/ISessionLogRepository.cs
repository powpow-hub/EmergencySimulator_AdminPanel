using EmergencySimulator.AdminPanel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public interface ISessionLogRepository : IRepository<SessionLog>
    {
        IEnumerable<SessionLog> GetByResultId(int resultId);
        Task<IEnumerable<SessionLog>> GetByResultIdAsync(int resultId);
        IEnumerable<SessionLog> GetByUserId(int userId);
    }
}
