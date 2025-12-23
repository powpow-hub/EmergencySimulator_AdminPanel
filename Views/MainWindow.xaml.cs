using EmergencySimulator.AdminPanel.Data;
using EmergencySimulator.AdminPanel.Services;
using EmergencySimulator.AdminPanel.ViewModels;
using System;
using System.Windows;

namespace EmergencySimulator.AdminPanel.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Устанавливаем MainViewModel как DataContext
            DataContext = new MainViewModel();

            // Отображаем информацию о текущем пользователе
            LoadAdminInfo();
        }

        private void LoadAdminInfo()
        {
            var admin = AuthenticationService.CurrentAdmin;
            if (admin != null)
            {
                AdminNameText.Text = admin.FullName;
                AdminUsernameText.Text = $"@{admin.Username}";

                // Обновляем заголовок окна
                Title = $"Панель администратора - {admin.FullName}";
            }
            else
            {
                AdminNameText.Text = "Неизвестный пользователь";
                AdminUsernameText.Text = "@unknown";
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы действительно хотите выйти из системы?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Очищаем сессию
                    using (var context = new DatabaseContext())
                    using (var unitOfWork = new UnitOfWork(context))
                    using (var authService = new AuthenticationService(unitOfWork))
                    {
                        authService.Logout();
                    }

                    // Закрываем главное окно
                    this.Hide();

                    // Открываем окно входа
                    var loginWindow = new LoginWindow();
                    if (loginWindow.ShowDialog() == true)
                    {
                        // Если вход успешен, обновляем информацию и показываем окно
                        LoadAdminInfo();
                        this.Show();
                    }
                    else
                    {
                        // Если вход отменен, закрываем приложение
                        Application.Current.Shutdown();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ошибка при выходе: {ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    // В случае ошибки показываем окно обратно
                    this.Show();
                }
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var changePasswordWindow = new ChangePasswordWindow(
                    isFirstLogin: false,
                    canCancel: true);

                if (changePasswordWindow.ShowDialog() == true)
                {
                    // Пароль успешно изменен
                    MessageBox.Show(
                        "Учетные данные успешно обновлены!\n\n" +
                        "Обратите внимание, что при следующем входе нужно будет использовать новые данные.",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Обновляем информацию о пользователе на экране
                    LoadAdminInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при изменении пароля: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Очищаем ресурсы DataContext если это IDisposable
            if (DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
