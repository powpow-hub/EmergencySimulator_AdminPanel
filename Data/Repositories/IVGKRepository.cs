using EmergencySimulator.AdminPanel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public interface IVGKRepository : IRepository<VGKMember>
    {
        IEnumerable<VGKMember> GetAllWithStaff();
        Task<IEnumerable<VGKMember>> GetAllWithStaffAsync();
        VGKMember GetCommander();
        Task<VGKMember> GetCommanderAsync();
        IEnumerable<VGKMember> GetOnDutyMembers();
        IEnumerable<VGKMember> FilterMembers(string searchText, string role, bool onDutyOnly);
    }
}
