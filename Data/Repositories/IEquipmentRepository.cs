using EmergencySimulator.AdminPanel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public interface IEquipmentRepository : IRepository<Equipment>
    {
        IEnumerable<Equipment> GetAvailableEquipment();
        Task<IEnumerable<Equipment>> GetAvailableEquipmentAsync();
        IEnumerable<Equipment> GetByType(string equipmentType);
        IEnumerable<Equipment> GetByLocation(string location);
        IEnumerable<Equipment> SearchEquipment(string searchText);
    }
}
