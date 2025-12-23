using EmergencySimulator.AdminPanel.Data;
using EmergencySimulator.AdminPanel.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EmergencySimulator.AdminPanel.Utils
{
    public static class DatabaseSeeder
    {
        public static void Initialize(DatabaseContext context)
        {
            // Проверяем, есть ли уже данные
            if (context.Users.Any())
            {
                return; // БД уже заполнена
            }

            // Добавляем администратора по умолчанию
            var adminUser = new User
            {
                Name = "Администратор",
                Surname = "Системы",
                Position = "Администратор",
                CreatedAt = DateTime.Now
            };
            SetPassword(adminUser, "admin123");
            context.Users.Add(adminUser);

            // Добавляем списки оповещения
            var list1 = new NotificationList
            {
                ListName = "Список №1",
                Description = "Первоочередное оповещение при аварии",
                Priority = 1
            };
            context.NotificationLists.Add(list1);

            var list2 = new NotificationList
            {
                ListName = "Список №2",
                Description = "Дополнительное оповещение",
                Priority = 2
            };
            context.NotificationLists.Add(list2);

            context.SaveChanges();

            // Добавляем персонал
            var staff = new[]
            {
                new Staff { Name = "Иван", Surname = "Петров", Position = "Главный инженер", Phone = "+7-900-123-45-67", ListID = list1.ListID, IsActive = true },
                new Staff { Name = "Сергей", Surname = "Иванов", Position = "Начальник службы ОТ", Phone = "+7-900-123-45-68", ListID = list1.ListID, IsActive = true },
                new Staff { Name = "Алексей", Surname = "Сидоров", Position = "Командир ВГК", Phone = "+7-900-123-45-69", ListID = list1.ListID, IsActive = true },
                new Staff { Name = "Михаил", Surname = "Козлов", Position = "Спасатель", Phone = "+7-900-123-45-70", IsActive = true },
                new Staff { Name = "Дмитрий", Surname = "Морозов", Position = "Фельдшер", Phone = "+7-900-123-45-71", IsActive = true }
            };
            context.Staff.AddRange(staff);
            context.SaveChanges();

            // Добавляем членов ВГК
            var vgkMembers = new[]
            {
                new VGKMember { StaffID = staff[2].StaffID, Role = "Командир", IsCommander = true, IsOnDuty = true },
                new VGKMember { StaffID = staff[3].StaffID, Role = "Спасатель", IsCommander = false, IsOnDuty = true },
                new VGKMember { StaffID = staff[4].StaffID, Role = "Медик", IsCommander = false, IsOnDuty = true }
            };
            context.VGKMembers.AddRange(vgkMembers);

            // Добавляем оборудование
            var equipment = new[]
            {
                new Equipment { EquipmentName = "Рубильник Отвал Южный", EquipmentType = "Электрооборудование", Location = "Электрощитовая", IsAvailable = true },
                new Equipment { EquipmentName = "Рация начальника смены", EquipmentType = "Связь", Location = "Стол начальника", IsAvailable = true },
                new Equipment { EquipmentName = "Терминал ВГСЧ", EquipmentType = "Терминал", Location = "Рабочее место РЛА", IsAvailable = true },
                new Equipment { EquipmentName = "Поливочная машина №1", EquipmentType = "Техника", Location = "Парк техники", IsAvailable = true }
            };
            context.Equipments.AddRange(equipment);

            // Добавляем данные ПЛА - Позиция №3 "Пожар"
            var plaData = new[]
            {
                new PLAData { PositionNumber = "№3", PositionName = "Пожар", ActionStep = "Вызвать ВГСЧ", StepOrder = 1, TimeNormative = 120, IsCritical = true, Description = "Немедленный вызов военизированной горноспасательной части" },
                new PLAData { PositionNumber = "№3", PositionName = "Пожар", ActionStep = "Оповестить по Списку №1", StepOrder = 2, TimeNormative = 180, IsCritical = true, Description = "Оповещение ключевых должностных лиц" },
                new PLAData { PositionNumber = "№3", PositionName = "Пожар", ActionStep = "Отключить электросети в зоне аварии", StepOrder = 3, TimeNormative = 300, IsCritical = true, Description = "Отключение электропитания для предотвращения поражения током" },
                new PLAData { PositionNumber = "№3", PositionName = "Пожар", ActionStep = "Выдать письменное задание командиру ВГК", StepOrder = 4, TimeNormative = 300, IsCritical = false, Description = "Формирование задачи для командира ВГК" },
                new PLAData { PositionNumber = "№3", PositionName = "Пожар", ActionStep = "Начать ведение оперативного журнала", StepOrder = 5, TimeNormative = 300, IsCritical = false, Description = "Документирование всех действий и событий" },
                new PLAData { PositionNumber = "№3", PositionName = "Пожар", ActionStep = "Организовать отправку ВГК", StepOrder = 6, TimeNormative = 300, IsCritical = true, Description = "Сбор и отправка ВГК к месту аварии" },
                new PLAData { PositionNumber = "№3", PositionName = "Пожар", ActionStep = "Вызвать фельдшера", StepOrder = 7, TimeNormative = 600, IsCritical = false, Description = "Обеспечение медицинской помощи" },
                new PLAData { PositionNumber = "№3", PositionName = "Пожар", ActionStep = "Отправить поливочную машину", StepOrder = 8, TimeNormative = 300, IsCritical = false, Description = "Дополнительное средство пожаротушения" }
            };
            context.PLA_Data.AddRange(plaData);

            context.SaveChanges();
        }

        private static void SetPassword(User user, string password)
        {
            // Генерация соли
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            user.Salt = Convert.ToBase64String(saltBytes);

            // Хеширование пароля с солью
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password + user.Salt);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                user.PasswordHash = Convert.ToBase64String(hashBytes);
            }
        }
    }
}
