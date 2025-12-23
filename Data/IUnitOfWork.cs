using EmergencySimulator.AdminPanel.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data
{
    public interface IUnitOfWork : IDisposable
    {
        // Специализированные репозитории
        IUserRepository Users { get; }
        IStaffRepository Staff { get; }
        IVGKRepository VGKMembers { get; }
        ITrainingResultRepository TrainingResults { get; }
        IPLADataRepository PLAData { get; }
        IEquipmentRepository Equipment { get; }
        INotificationListRepository NotificationLists { get; }
        ISessionActionRepository SessionActions { get; }
        ISessionLogRepository SessionLogs { get; }
        IEmergencyCallRepository EmergencyCalls { get; }
        IAdminRepository Admins { get; }

        // Методы сохранения
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
