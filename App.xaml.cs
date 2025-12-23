using EmergencySimulator.AdminPanel.Data;
using EmergencySimulator.AdminPanel.Services;
using EmergencySimulator.AdminPanel.Views;
using System.Windows;

namespace EmergencySimulator.AdminPanel
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Отключаем автоматическое создание MainWindow
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Показываем окно входа
            var loginWindow = new LoginWindow();

            if (loginWindow.ShowDialog() == true)
            {
                // Проверяем, используются ли учетные данные по умолчанию
                using (var context = new DatabaseContext())
                using (var unitOfWork = new UnitOfWork(context))
                using (var authService = new AuthenticationService(unitOfWork))
                {
                    if (authService.IsDefaultCredentials())
                    {
                        // Показываем окно смены пароля
                        var changePasswordWindow = new ChangePasswordWindow(
                            isFirstLogin: true,
                            canCancel: false);

                        if (changePasswordWindow.ShowDialog() != true)
                        {
                            // Если пользователь отказался менять пароль, выходим
                            MessageBox.Show(
                                "Для продолжения работы необходимо изменить учетные данные по умолчанию.",
                                "Требуется изменение данных",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                            Shutdown();
                            return;
                        }

                        // После смены пароля нужно разлогиниться и войти заново
                        authService.Logout();

                        MessageBox.Show(
                            "Учетные данные успешно изменены!\n\n" +
                            "Пожалуйста, войдите в систему с новыми учетными данными.",
                            "Вход в систему",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Показываем окно входа снова
                        var newLoginWindow = new LoginWindow();
                        if (newLoginWindow.ShowDialog() != true)
                        {
                            Shutdown();
                            return;
                        }
                    }
                }

                // Если авторизация успешна, открываем главное окно
                var mainWindow = new MainWindow();

                // Устанавливаем режим завершения при закрытии главного окна
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                MainWindow = mainWindow;

                mainWindow.Show();
            }
            else
            {
                // Если авторизация отменена, закрываем приложение
                Shutdown();
            }
        }
    }
}
