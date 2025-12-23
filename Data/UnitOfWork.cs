using EmergencySimulator.AdminPanel.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace EmergencySimulator.AdminPanel.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;

        // Приватные поля для ленивой инициализации
        private IUserRepository _users;
        private IStaffRepository _staff;
        private IVGKRepository _vgkMembers;
        private ITrainingResultRepository _trainingResults;
        private IPLADataRepository _plaData;
        private IEquipmentRepository _equipment;
        private INotificationListRepository _notificationLists;
        private ISessionActionRepository _sessionActions;
        private ISessionLogRepository _sessionLogs;
        private IEmergencyCallRepository _emergencyCalls;
        private IAdminRepository _admins;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Публичные свойства с ленивой инициализацией
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IStaffRepository Staff => _staff ??= new StaffRepository(_context);
        public IVGKRepository VGKMembers => _vgkMembers ??= new VGKRepository(_context);
        public ITrainingResultRepository TrainingResults => _trainingResults ??= new TrainingResultRepository(_context);
        public IPLADataRepository PLAData => _plaData ??= new PLADataRepository(_context);
        public IEquipmentRepository Equipment => _equipment ??= new EquipmentRepository(_context);
        public INotificationListRepository NotificationLists => _notificationLists ??= new NotificationListRepository(_context);
        public ISessionActionRepository SessionActions => _sessionActions ??= new SessionActionRepository(_context);
        public ISessionLogRepository SessionLogs => _sessionLogs ??= new SessionLogRepository(_context);
        public IEmergencyCallRepository EmergencyCalls => _emergencyCalls ??= new EmergencyCallRepository(_context);
        public IAdminRepository Admins => _admins ??= new AdminRepository(_context);

        public int SaveChanges()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Логирование ошибки можно добавить здесь
                throw new Exception("Ошибка сохранения изменений в базу данных", ex);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Логирование ошибки можно добавить здесь
                throw new Exception("Ошибка сохранения изменений в базу данных", ex);
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
