using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using EmergencySimulator.AdminPanel.Commands;
using EmergencySimulator.AdminPanel.Data;
using EmergencySimulator.AdminPanel.Models;

namespace EmergencySimulator.AdminPanel.ViewModels
{
    public class UsersViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private ObservableCollection<User> _users;
        private User _selectedUser;
        private string _searchText;
        private bool _isLoading;

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalUsers));
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsUserSelected));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                SearchUsers();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool IsUserSelected => SelectedUser != null;
        public int TotalUsers => Users?.Count ?? 0;

        // Команды
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand ViewStatisticsCommand { get; }
        public ICommand AdminResetPasswordCommand { get; }

        public UsersViewModel()
        {
            _unitOfWork = new UnitOfWork(new DatabaseContext());

            AddUserCommand = new RelayCommand(_ => AddUser());
            EditUserCommand = new RelayCommand(_ => EditUser(), _ => SelectedUser != null);
            DeleteUserCommand = new RelayCommand(_ => DeleteUser(), _ => SelectedUser != null);
            RefreshCommand = new RelayCommand(_ => LoadUsers());
            ResetPasswordCommand = new RelayCommand(_ => ResetPassword(), _ => SelectedUser != null);
            ViewStatisticsCommand = new RelayCommand(_ => ViewStatistics(), _ => SelectedUser != null);
            AdminResetPasswordCommand = new RelayCommand(_ => AdminResetPassword(), _ => SelectedUser != null);

            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                IsLoading = true;
                var users = _unitOfWork.Users.GetAllWithDetails();
                Users = new ObservableCollection<User>(users);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void SearchUsers()
        {
            try
            {
                IsLoading = true;
                var users = _unitOfWork.Users.SearchUsers(SearchText);
                Users = new ObservableCollection<User>(users);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddUser()
        {
            try
            {
                var newUser = new User
                {
                    Name = "Новый",
                    Surname = "Пользователь",
                    Position = "Начальник смены",
                    CreatedAt = DateTime.Now
                };

                string tempPassword = GenerateTemporaryPassword();
                SetPassword(newUser, tempPassword);

                _unitOfWork.Users.Add(newUser);
                _unitOfWork.SaveChanges();
                LoadUsers();

                MessageBox.Show(
                    $"Пользователь добавлен успешно!\n\n" +
                    $"Временный пароль: {tempPassword}\n\n" +
                    $"Пожалуйста, запишите пароль и передайте пользователю.",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditUser()
        {
            if (SelectedUser == null) return;

            try
            {
                _unitOfWork.Users.Update(SelectedUser);
                _unitOfWork.SaveChanges();

                MessageBox.Show("Изменения сохранены!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteUser()
        {
            if (SelectedUser == null) return;

            int trainingCount = SelectedUser.TrainingResults?.Count ?? 0;

            var message = trainingCount > 0
                ? $"Удалить пользователя {SelectedUser.Surname} {SelectedUser.Name}?\n\n" +
                  $"ВНИМАНИЕ: Будут удалены все связанные данные:\n" +
                  $"- Результаты тренировок: {trainingCount}\n" +
                  $"- Логи сессий\n" +
                  $"- История действий"
                : $"Удалить пользователя {SelectedUser.Surname} {SelectedUser.Name}?";

            var result = MessageBox.Show(
                message,
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _unitOfWork.Users.Remove(SelectedUser);
                    _unitOfWork.SaveChanges();
                    LoadUsers();

                    MessageBox.Show("Пользователь удалён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ResetPassword()
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show(
                $"Сбросить пароль для пользователя {SelectedUser.Surname} {SelectedUser.Name}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    string newPassword = GenerateTemporaryPassword();
                    SetPassword(SelectedUser, newPassword);

                    _unitOfWork.Users.Update(SelectedUser);
                    _unitOfWork.SaveChanges();

                    MessageBox.Show(
                        $"Пароль успешно сброшен!\n\n" +
                        $"Новый временный пароль: {newPassword}\n\n" +
                        $"Пожалуйста, передайте пароль пользователю.",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сброса пароля: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AdminResetPassword()
        {
            if (SelectedUser == null) return;

            try
            {
                var resetWindow = new Views.ResetUserPasswordWindow(SelectedUser);

                if (resetWindow.ShowDialog() == true)
                {
                    // Пароль успешно сброшен, обновляем список
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка сброса пароля: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private void ViewStatistics()
        {
            if (SelectedUser == null) return;

            try
            {
                var trainingResults = _unitOfWork.TrainingResults
                    .GetByUserId(SelectedUser.UserID)
                    .ToList();

                int totalSessions = trainingResults.Count;
                int completedSessions = trainingResults.Count(r => r.Status == "Завершено");
                double avgScore = trainingResults.Any()
                    ? trainingResults.Average(r => r.TotalScore)
                    : 0;

                // CriticalErrorsCount — double, суммируем напрямую
                double totalCritical = trainingResults.Sum(r => r.CriticalErrorsCount);

                // SignificantErrorsCount — string, парсим безопасно
                int totalSignificant = trainingResults.Sum(r =>
                    int.TryParse(r.SignificantErrorsCount, out int val) ? val : 0);

                int totalTimeViolations = trainingResults.Sum(r => r.TimeViolationsCount);
                double totalErrors = totalCritical + totalSignificant;

                var stats = $"СТАТИСТИКА ПОЛЬЗОВАТЕЛЯ\n" +
                            $"═══════════════════════════════\n\n" +
                            $"ФИО: {SelectedUser.Surname} {SelectedUser.Name} {SelectedUser.MiddleName}\n" +
                            $"Должность: {SelectedUser.Position}\n\n" +
                            $"Дата создания: {SelectedUser.CreatedAt:dd.MM.yyyy}\n" +
                            $"Последний вход: {(SelectedUser.LastLogin.HasValue ? SelectedUser.LastLogin.Value.ToString("dd.MM.yyyy HH:mm") : "Никогда")}\n\n" +
                            $"РЕЗУЛЬТАТЫ ТРЕНИРОВОК:\n" +
                            $"───────────────────────────────\n" +
                            $"Всего тренировок: {totalSessions}\n" +
                            $"Завершено: {completedSessions}\n" +
                            $"Средний балл: {avgScore:F1}\n" +
                            $"Всего ошибок: {totalErrors:F0}\n" +
                            $"  ├─ Критических: {totalCritical:F0}\n" +
                            $"  ├─ Значительных: {totalSignificant}\n" +
                            $"  └─ Нарушений времени: {totalTimeViolations}";

                MessageBox.Show(stats, "Статистика пользователя",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка получения статистики: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void SetPassword(User user, string password)
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            user.Salt = Convert.ToBase64String(saltBytes);

            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password + user.Salt);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                user.PasswordHash = Convert.ToBase64String(hashBytes);
            }
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
