using EmergencySimulator.AdminPanel.Data;
using EmergencySimulator.AdminPanel.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace EmergencySimulator.AdminPanel.Views
{
    public partial class ResetUserPasswordWindow : Window
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly User _user;
        public string NewPassword { get; private set; }

        public ResetUserPasswordWindow(User user)
        {
            InitializeComponent();

            _unitOfWork = new UnitOfWork(new DatabaseContext());
            _user = user ?? throw new ArgumentNullException(nameof(user));

            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            UserNameText.Text = $"{_user.Surname} {_user.Name} {_user.MiddleName}".Trim();
            UserPositionText.Text = _user.Position;
            UserCreatedText.Text = _user.CreatedAt.ToString("dd.MM.yyyy HH:mm");
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Вы уверены, что хотите сбросить пароль для пользователя:\n\n" +
                $"{_user.Surname} {_user.Name}?\n\n" +
                $"Текущий пароль будет заменён на новый временный.",
                "Подтверждение сброса пароля",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                ResetPassword();
            }
        }

        private void ResetPassword()
        {
            try
            {
                // Генерируем новый временный пароль
                NewPassword = GenerateTemporaryPassword();

                // Генерируем новую соль
                byte[] saltBytes = new byte[16];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(saltBytes);
                }
                _user.Salt = Convert.ToBase64String(saltBytes);

                // Хешируем новый пароль
                using (var sha256 = SHA256.Create())
                {
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(NewPassword + _user.Salt);
                    byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                    _user.PasswordHash = Convert.ToBase64String(hashBytes);
                }

                // Сохраняем изменения
                _unitOfWork.Users.Update(_user);
                _unitOfWork.SaveChanges();

                // Показываем новый пароль
                var notifyUser = NotifyUserCheckBox.IsChecked == true;
                var message = $"Пароль успешно сброшен!\n\n" +
                             $"Новый временный пароль:\n" +
                             $"━━━━━━━━━━━━━━━━━━━━\n" +
                             $"{NewPassword}\n" +
                             $"━━━━━━━━━━━━━━━━━━━━\n\n" +
                             $"Пользователь: {_user.Surname} {_user.Name}\n\n" +
                             $"⚠ ВАЖНО:\n" +
                             $"• Запишите этот пароль\n" +
                             $"• Передайте его пользователю безопасным способом\n" +
                             $"• Рекомендуйте пользователю сменить пароль после входа\n";

                if (notifyUser)
                {
                    message += $"\n📧 Уведомление пользователю не отправлено (функция в разработке)";
                }

                MessageBox.Show(
                    message,
                    "Пароль сброшен",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
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

        private string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
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
            _unitOfWork?.Dispose();
            base.OnClosed(e);
        }
    }
}
