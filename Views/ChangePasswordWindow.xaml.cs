using System;
using EmergencySimulator.AdminPanel.Data;
using EmergencySimulator.AdminPanel.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace EmergencySimulator.AdminPanel.Views
{
    public partial class ChangePasswordWindow : Window
    {
        private readonly AuthenticationService _authService;
        private readonly bool _isFirstLogin;
        private readonly bool _canCancel;

        public bool CredentialsChanged { get; private set; } = false;

        /// <summary>
        /// Конструктор для окна смены пароля
        /// </summary>
        /// <param name="isFirstLogin">Первый ли это вход (требует обязательной смены)</param>
        /// <param name="canCancel">Можно ли отменить операцию</param>
        public ChangePasswordWindow(bool isFirstLogin = false, bool canCancel = true)
        {
            InitializeComponent();

            var context = new DatabaseContext();
            var unitOfWork = new UnitOfWork(context);
            _authService = new AuthenticationService(unitOfWork);

            _isFirstLogin = isFirstLogin;
            _canCancel = canCancel;

            // Настройка интерфейса
            if (!canCancel)
            {
                CloseBtn.Visibility = Visibility.Collapsed;
            }

            if (isFirstLogin)
            {
                WarningPanel.Visibility = Visibility.Visible;
            }

            // Устанавливаем текущего пользователя
            if (AuthenticationService.CurrentAdmin != null)
            {
                CurrentUserText.Text = AuthenticationService.CurrentAdmin.Username;
                NewUsernameTextBox.Text = AuthenticationService.CurrentAdmin.Username;
            }

            // Фокус на поле старого пароля
            Loaded += (s, e) => OldPasswordBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
        }

        private void SaveChanges()
        {
            ErrorPanel.Visibility = Visibility.Collapsed;

            var currentAdmin = AuthenticationService.CurrentAdmin;
            if (currentAdmin == null)
            {
                ShowError("Ошибка: администратор не авторизован");
                return;
            }

            // Валидация полей
            string newUsername = NewUsernameTextBox.Text.Trim();
            string oldPassword = OldPasswordBox.Password;
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // Проверка нового логина
            if (string.IsNullOrWhiteSpace(newUsername))
            {
                ShowError("Пожалуйста, введите новый логин");
                NewUsernameTextBox.Focus();
                return;
            }

            if (newUsername.Length < 3)
            {
                ShowError("Логин должен содержать минимум 3 символа");
                NewUsernameTextBox.Focus();
                return;
            }

            // Проверка старого пароля
            if (string.IsNullOrWhiteSpace(oldPassword))
            {
                ShowError("Пожалуйста, введите текущий пароль");
                OldPasswordBox.Focus();
                return;
            }

            // Проверка нового пароля
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ShowError("Пожалуйста, введите новый пароль");
                NewPasswordBox.Focus();
                return;
            }

            if (newPassword.Length < 6)
            {
                ShowError("Новый пароль должен содержать минимум 6 символов");
                NewPasswordBox.Focus();
                return;
            }

            // Проверка совпадения паролей
            if (newPassword != confirmPassword)
            {
                ShowError("Пароли не совпадают. Пожалуйста, проверьте введенные данные.");
                ConfirmPasswordBox.Focus();
                return;
            }

            // Проверка, что новый пароль отличается от старого
            if (newPassword == oldPassword)
            {
                ShowError("Новый пароль должен отличаться от текущего");
                NewPasswordBox.Focus();
                return;
            }

            try
            {
                // Проверяем текущий пароль
                if (!_authService.ValidateCurrentPassword(currentAdmin.AdminID, oldPassword))
                {
                    ShowError("Неверный текущий пароль");
                    OldPasswordBox.Focus();
                    return;
                }

                // Проверяем, не занят ли новый логин другим пользователем
                if (newUsername != currentAdmin.Username)
                {
                    if (_authService.IsUsernameExists(newUsername, currentAdmin.AdminID))
                    {
                        ShowError("Этот логин уже используется другим администратором");
                        NewUsernameTextBox.Focus();
                        return;
                    }
                }

                // Обновляем учетные данные
                if (_authService.UpdateAdminCredentials(
                    currentAdmin.AdminID,
                    newUsername,
                    oldPassword,
                    newPassword))
                {
                    CredentialsChanged = true;

                    MessageBox.Show(
                        "Учетные данные успешно изменены!\n\n" +
                        $"Новый логин: {newUsername}\n\n" +
                        "При следующем входе используйте новые учетные данные.",
                        "Успешно",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }
                else
                {
                    ShowError("Не удалось изменить учетные данные. Попробуйте еще раз.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorPanel.Visibility = Visibility.Visible;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_canCancel)
            {
                var result = MessageBox.Show(
                    "Для безопасности системы необходимо изменить учетные данные по умолчанию.\n\n" +
                    "Вы действительно хотите выйти без изменения данных?",
                    "Внимание",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CancelButton_Click(sender, e);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _authService?.Dispose();
            base.OnClosed(e);
        }
    }
}
