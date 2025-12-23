using EmergencySimulator.AdminPanel.Data;
using EmergencySimulator.AdminPanel.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace EmergencySimulator.AdminPanel.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthenticationService _authService;

        public LoginWindow()
        {
            InitializeComponent();

            var context = new DatabaseContext();
            var unitOfWork = new UnitOfWork(context);
            _authService = new AuthenticationService(unitOfWork);

            // Фокус на поле ввода логина
            Loaded += (s, e) => UsernameTextBox.Focus();

            // Инициализация базы данных
            context.InitializeDatabase();

            // Создание администратора по умолчанию, если его нет
            CreateDefaultAdminIfNotExists();
        }

        private void CreateDefaultAdminIfNotExists()
        {
            try
            {
                var context = new DatabaseContext();
                var unitOfWork = new UnitOfWork(context);

                // Проверяем, есть ли хоть один администратор
                var admins = unitOfWork.Admins.GetAll();
                if (!admins.Any())
                {
                    // Создаем администратора по умолчанию
                    _authService.CreateAdmin(
                        username: "admin",
                        password: "admin123",
                        fullName: "Администратор системы",
                        email: "admin@emergency.local"
                    );

                    MessageBox.Show(
                        "Создан администратор по умолчанию:\n\n" +
                        "Логин: admin\n" +
                        "Пароль: admin123\n\n" +
                        "Пожалуйста, измените пароль после первого входа!",
                        "Первый запуск",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка инициализации: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            PerformLogin();
        }

        private void InputField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformLogin();
            }
        }

        private void PerformLogin()
        {
            // Скрываем сообщение об ошибке
            ErrorPanel.Visibility = Visibility.Collapsed;

            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            // Валидация полей
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Пожалуйста, введите имя пользователя");
                UsernameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Пожалуйста, введите пароль");
                PasswordBox.Focus();
                return;
            }

            // Попытка входа
            try
            {
                if (_authService.Login(username, password))
                {
                    // Успешный вход
                    DialogResult = true;
                    Close();
                }
                else
                {
                    // Неверные учетные данные
                    ShowError("Неверное имя пользователя или пароль");
                    PasswordBox.Clear();
                    PasswordBox.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка входа: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorPanel.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
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
