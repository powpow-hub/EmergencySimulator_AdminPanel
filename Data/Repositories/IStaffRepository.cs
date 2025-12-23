using EmergencySimulator.AdminPanel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public interface IStaffRepository : IRepository<Staff>
    {
        IEnumerable<Staff> GetAllWithDetails();
        Task<IEnumerable<Staff>> GetAllWithDetailsAsync();
        IEnumerable<Staff> SearchStaff(string searchText);
        Task<IEnumerable<Staff>> SearchStaffAsync(string searchText);
        IEnumerable<Staff> GetActiveStaff();
        IEnumerable<Staff> GetStaffByNotificationList(int listId);
    }
}
