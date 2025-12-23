using EmergencySimulator.AdminPanel.Data;
using EmergencySimulator.AdminPanel.Models;
using System;

namespace EmergencySimulator.AdminPanel.Services
{
    /// <summary>
    /// Сервис для управления аутентификацией администраторов
    /// </summary>
    public class AuthenticationService : IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private static Admin _currentAdmin;

        public AuthenticationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Текущий авторизованный администратор
        /// </summary>
        public static Admin CurrentAdmin => _currentAdmin;

        /// <summary>
        /// Проверка, авторизован ли пользователь
        /// </summary>
        public static bool IsAuthenticated => _currentAdmin != null;

        /// <summary>
        /// Попытка входа в систему
        /// </summary>
        public bool Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return false;

                // Проверяем учетные данные
                if (_unitOfWork.Admins.ValidateCredentials(username, password))
                {
                    var admin = _unitOfWork.Admins.GetByUsername(username);
                    if (admin != null && admin.IsActive)
                    {
                        _currentAdmin = admin;

                        // Обновляем время последнего входа
                        _unitOfWork.Admins.UpdateLastLogin(admin.AdminID);
                        _unitOfWork.SaveChanges();

                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Выход из системы
        /// </summary>
        public void Logout()
        {
            _currentAdmin = null;
        }

        /// <summary>
        /// Создание нового администратора
        /// </summary>
        public bool CreateAdmin(string username, string password, string fullName, string email = null)
        {
            try
            {
                // Проверяем, существует ли уже такой логин
                var existing = _unitOfWork.Admins.GetByUsername(username);
                if (existing != null)
                    return false;

                // Генерируем соль
                byte[] saltBytes = new byte[16];
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    rng.GetBytes(saltBytes);
                }
                string salt = Convert.ToBase64String(saltBytes);

                // Хешируем пароль
                string passwordHash;
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
                    byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                    passwordHash = Convert.ToBase64String(hashBytes);
                }

                // Создаем нового администратора
                var admin = new Admin
                {
                    Username = username,
                    PasswordHash = passwordHash,
                    Salt = salt,
                    FullName = fullName,
                    Email = email,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    Role = "Admin"
                };

                _unitOfWork.Admins.Add(admin);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Изменение пароля администратора
        /// </summary>
        public bool ChangePassword(int adminId, string oldPassword, string newPassword)
        {
            try
            {
                var admin = _unitOfWork.Admins.GetById(adminId);
                if (admin == null)
                    return false;

                // Проверяем старый пароль
                if (!_unitOfWork.Admins.ValidateCredentials(admin.Username, oldPassword))
                    return false;

                // Генерируем новую соль
                byte[] saltBytes = new byte[16];
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    rng.GetBytes(saltBytes);
                }
                admin.Salt = Convert.ToBase64String(saltBytes);

                // Хешируем новый пароль
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(newPassword + admin.Salt);
                    byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                    admin.PasswordHash = Convert.ToBase64String(hashBytes);
                }

                _unitOfWork.Admins.Update(admin);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Обновление логина и пароля администратора
        /// </summary>
        public bool UpdateAdminCredentials(int adminId, string newUsername, string oldPassword, string newPassword)
        {
            try
            {
                var admin = _unitOfWork.Admins.GetById(adminId);
                if (admin == null)
                    return false;

                // Проверяем старый пароль
                if (!_unitOfWork.Admins.ValidateCredentials(admin.Username, oldPassword))
                    return false;

                // Обновляем логин
                admin.Username = newUsername;

                // Генерируем новую соль
                byte[] saltBytes = new byte[16];
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    rng.GetBytes(saltBytes);
                }
                admin.Salt = Convert.ToBase64String(saltBytes);

                // Хешируем новый пароль
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(newPassword + admin.Salt);
                    byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                    admin.PasswordHash = Convert.ToBase64String(hashBytes);
                }

                _unitOfWork.Admins.Update(admin);
                _unitOfWork.SaveChanges();

                // Обновляем текущего администратора в памяти
                _currentAdmin = admin;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка текущего пароля
        /// </summary>
        public bool ValidateCurrentPassword(int adminId, string password)
        {
            try
            {
                var admin = _unitOfWork.Admins.GetById(adminId);
                if (admin == null)
                    return false;

                return _unitOfWork.Admins.ValidateCredentials(admin.Username, password);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка существования логина
        /// </summary>
        public bool IsUsernameExists(string username, int excludeAdminId = 0)
        {
            try
            {
                var admin = _unitOfWork.Admins.GetByUsername(username);
                return admin != null && admin.AdminID != excludeAdminId;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка, является ли это первым входом с учетными данными по умолчанию
        /// </summary>
        public bool IsDefaultCredentials()
        {
            var admin = CurrentAdmin;
            if (admin == null)
                return false;

            // Проверяем, является ли это администратором по умолчанию
            return admin.Username.Equals("admin", StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
        }
    }
}
