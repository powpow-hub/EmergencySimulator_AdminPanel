using EmergencySimulator.AdminPanel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public interface INotificationListRepository : IRepository<NotificationList>
    {
        IEnumerable<NotificationList> GetAllWithMembers();
        Task<IEnumerable<NotificationList>> GetAllWithMembersAsync();
        NotificationList GetByIdWithMembers(int id);
        Task<NotificationList> GetByIdWithMembersAsync(int id);
        IEnumerable<NotificationList> GetByPriority(int priority);
    }
}
