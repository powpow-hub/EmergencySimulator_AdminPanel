using EmergencySimulator.AdminPanel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public interface IEmergencyCallRepository : IRepository<EmergencyCall>
    {
        IEnumerable<EmergencyCall> GetByResultId(int resultId);
        Task<IEnumerable<EmergencyCall>> GetByResultIdAsync(int resultId);
        IEnumerable<EmergencyCall> GetIncompleteCallsByResultId(int resultId);
    }
}
