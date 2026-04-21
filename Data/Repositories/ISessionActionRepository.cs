using EmergencySimulator.AdminPanel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public interface ISessionActionRepository : IRepository<SessionAction>
    {
        IEnumerable<SessionAction> GetByResultId(int resultId);
        Task<IEnumerable<SessionAction>> GetByResultIdAsync(int resultId);
        IEnumerable<SessionAction> GetErrorActions(int resultId);
        IEnumerable<SessionAction> GetCriticalErrorActions(int resultId);
    }
}
