using EmergencySimulator.AdminPanel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByIdWithDetails(int id);
        Task<User> GetByIdWithDetailsAsync(int id);
        IEnumerable<User> GetAllWithDetails();
        Task<IEnumerable<User>> GetAllWithDetailsAsync();
        IEnumerable<User> SearchUsers(string searchText);
        Task<IEnumerable<User>> SearchUsersAsync(string searchText);
        User GetByUsername(string surname, string name);
    }
}
