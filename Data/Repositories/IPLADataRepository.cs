using EmergencySimulator.AdminPanel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public interface IPLADataRepository : IRepository<PLAData>
    {
        IEnumerable<PLAData> GetByPosition(string positionNumber);
        Task<IEnumerable<PLAData>> GetByPositionAsync(string positionNumber);
        IEnumerable<PLAData> GetCriticalActions();
        IEnumerable<PLAData> SearchPLA(string searchText);
    }
}
