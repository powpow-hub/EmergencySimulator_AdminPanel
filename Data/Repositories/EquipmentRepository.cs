using EmergencySimulator.AdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data.Repositories
{
    public class EquipmentRepository : Repository<Equipment>, IEquipmentRepository
    {
        public EquipmentRepository(DatabaseContext context) : base(context) { }

        public IEnumerable<Equipment> GetAvailableEquipment()
        {
            return _context.Equipments
                .Where(e => e.IsAvailable)
                .ToList();
        }

        public async Task<IEnumerable<Equipment>> GetAvailableEquipmentAsync()
        {
            return await _context.Equipments
                .Where(e => e.IsAvailable)
                .ToListAsync();
        }

        public IEnumerable<Equipment> GetByType(string equipmentType)
        {
            return _context.Equipments
                .Where(e => e.EquipmentType == equipmentType)
                .ToList();
        }

        public IEnumerable<Equipment> GetByLocation(string location)
        {
            return _context.Equipments
                .Where(e => e.Location == location)
                .ToList();
        }

        public IEnumerable<Equipment> SearchEquipment(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAll();

            return _context.Equipments
                .Where(e => e.EquipmentName.Contains(searchText) ||
                           e.EquipmentType.Contains(searchText) ||
                           (e.Location != null && e.Location.Contains(searchText)))
                .ToList();
        }
    }
}
