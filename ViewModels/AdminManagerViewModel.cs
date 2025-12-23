using EmergencySimulator.AdminPanel.Commands;
using EmergencySimulator.AdminPanel.Data;
using EmergencySimulator.AdminPanel.Models;
using EmergencySimulator.AdminPanel.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace EmergencySimulator.AdminPanel.ViewModels
{
    /// <summary>
    /// ViewModel для управления администраторами системы
    /// Добавьте эту ViewModel как отдельный пункт меню в навигации
    /// </summary>
    public class AdminManagerViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AuthenticationService _authService;
        private ObservableCollection<Admin> _administrators;
        private Admin _selectedAdmin;
        private string _searchText;

        public ObservableCollection<Admin> Administrators
        {
            get => _administrators;
            set
            {
                _administrators = value;
                OnPropertyChanged();
            }
        }

        public Admin SelectedAdmin
        {
            get => _selectedAdmin;
            set
            {
                _selectedAdmin = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                SearchAdministrators();
            }
        }

        // Команды
        public ICommand AddAdminCommand { get; }
        public ICommand EditAdminCommand { get; }
        public ICommand DeleteAdminCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand ToggleActiveCommand { get; }

        public AdminManagerViewModel()
        {
            _unitOfWork = new UnitOfWork(new DatabaseContext());
            _authService = new AuthenticationService(_unitOfWork);

            AddAdminCommand = new RelayCommand(_ => AddAdmin());
            EditAdminCommand = new RelayCommand(_ => EditAdmin(), _ => SelectedAdmin != null);
            DeleteAdminCommand = new RelayCommand(_ => DeleteAdmin(), _ => SelectedAdmin != null);
            RefreshCommand = new RelayCommand(_ => LoadAdministrators());
            ChangePasswordCommand = new RelayCommand(_ => ChangePassword(), _ => SelectedAdmin != null);
            ToggleActiveCommand = new RelayCommand(_ => ToggleActive(), _ => SelectedAdmin != null);

            LoadAdministrators();
        }

        private void LoadAdministrators()
        {
            try
            {
                var admins = _unitOfWork.Admins.GetAll();
                Administrators = new ObservableCollection<Admin>(admins);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchAdministrators()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    LoadAdministrators();
                    return;
                }

                var filtered = _unitOfWork.Admins.GetAll()
                    .Where(a => a.Username.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               a.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                               (a.Email != null && a.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                Administrators = new ObservableCollection<Admin>(filtered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAdmin()
        {
            try
            {
                // Здесь можно создать отдельное диалоговое окно для ввода данных
                // Для примера используем простой MessageBox
                var username = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите имя пользователя:", "Новый администратор", "");

                if (string.IsNullOrWhiteSpace(username))
                    return;

                var fullName = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите полное имя:", "Новый администратор", "");

                if (string.IsNullOrWhiteSpace(fullName))
                    return;

                var password = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите пароль:", "Новый администратор", "");

                if (string.IsNullOrWhiteSpace(password))
                    return;

                var email = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите email (необязательно):", "Новый администратор", "");

                if (_authService.CreateAdmin(username, password, fullName,
                    string.IsNullOrWhiteSpace(email) ? null : email))
                {
                    LoadAdministrators();
                    MessageBox.Show("Администратор успешно создан!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка создания администратора. Возможно, такой логин уже существует.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditAdmin()
        {
            if (SelectedAdmin == null) return;

            try
            {
                var fullName = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите полное имя:", "Редактирование", SelectedAdmin.FullName);

                if (string.IsNullOrWhiteSpace(fullName))
                    return;

                var email = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите email:", "Редактирование", SelectedAdmin.Email ?? "");

                SelectedAdmin.FullName = fullName;
                SelectedAdmin.Email = string.IsNullOrWhiteSpace(email) ? null : email;

                _unitOfWork.Admins.Update(SelectedAdmin);
                _unitOfWork.SaveChanges();

                LoadAdministrators();
                MessageBox.Show("Изменения сохранены!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteAdmin()
        {
            if (SelectedAdmin == null) return;

            // Проверяем, не пытается ли пользователь удалить сам себя
            if (SelectedAdmin.AdminID == AuthenticationService.CurrentAdmin?.AdminID)
            {
                MessageBox.Show("Вы не можете удалить собственную учетную запись!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить администратора {SelectedAdmin.FullName}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _unitOfWork.Admins.Remove(SelectedAdmin);
                    _unitOfWork.SaveChanges();
                    LoadAdministrators();

                    MessageBox.Show("Администратор удалён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ChangePassword()
        {
            if (SelectedAdmin == null) return;

            try
            {
                var oldPassword = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите текущий пароль:", "Смена пароля", "");

                if (string.IsNullOrWhiteSpace(oldPassword))
                    return;

                var newPassword = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите новый пароль:", "Смена пароля", "");

                if (string.IsNullOrWhiteSpace(newPassword))
                    return;

                var confirmPassword = Microsoft.VisualBasic.Interaction.InputBox(
                    "Подтвердите новый пароль:", "Смена пароля", "");

                if (newPassword != confirmPassword)
                {
                    MessageBox.Show("Пароли не совпадают!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (_authService.ChangePassword(SelectedAdmin.AdminID, oldPassword, newPassword))
                {
                    MessageBox.Show("Пароль успешно изменён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Неверный текущий пароль!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка смены пароля: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToggleActive()
        {
            if (SelectedAdmin == null) return;

            // Проверяем, не пытается ли пользователь деактивировать сам себя
            if (SelectedAdmin.AdminID == AuthenticationService.CurrentAdmin?.AdminID)
            {
                MessageBox.Show("Вы не можете деактивировать собственную учетную запись!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SelectedAdmin.IsActive = !SelectedAdmin.IsActive;
                _unitOfWork.Admins.Update(SelectedAdmin);
                _unitOfWork.SaveChanges();

                LoadAdministrators();

                string status = SelectedAdmin.IsActive ? "активирован" : "деактивирован";
                MessageBox.Show($"Администратор {status}!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка изменения статуса: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
            _authService?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
