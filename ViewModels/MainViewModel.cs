using System.Windows.Input;
using EmergencySimulator.AdminPanel.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Windows;

namespace EmergencySimulator.AdminPanel.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentViewModel;

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                // Dispose предыдущей ViewModel если она реализует IDisposable
                if (_currentViewModel is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        // Команды навигации
        public ICommand NavigateToUsersCommand { get; }
        public ICommand NavigateToStaffCommand { get; }
        public ICommand NavigateToPLACommand { get; }
        public ICommand NavigateToTrainingResultsCommand { get; }
        public ICommand NavigateToEquipmentCommand { get; }
        public ICommand NavigateToVGKCommand { get; }
        public ICommand NavigateToNotificationListsCommand { get; }

        public MainViewModel()
        {
            // Инициализация команд навигации
            NavigateToUsersCommand = new RelayCommand(_ => NavigateToUsers());
            NavigateToStaffCommand = new RelayCommand(_ => NavigateToStaff());
            NavigateToPLACommand = new RelayCommand(_ => NavigateToPLA());
            NavigateToTrainingResultsCommand = new RelayCommand(_ => NavigateToTrainingResults());
            NavigateToEquipmentCommand = new RelayCommand(_ => NavigateToEquipment());
            NavigateToVGKCommand = new RelayCommand(_ => NavigateToVGK());
            NavigateToNotificationListsCommand = new RelayCommand(_ => NavigateToNotificationLists());

            // Начальная страница
            CurrentViewModel = new UsersViewModel();
        }

        private void NavigateToUsers()
        {
            try
            {
                CurrentViewModel = new UsersViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка навигации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToStaff()
        {
            try
            {
                CurrentViewModel = new StaffViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка навигации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToPLA()
        {
            try
            {
                CurrentViewModel = new PLAViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка навигации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToTrainingResults()
        {
            try
            {
                CurrentViewModel = new TrainingResultsViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка навигации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToEquipment()
        {
            try
            {
                CurrentViewModel = new EquipmentViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка навигации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToVGK()
        {
            try
            {
                CurrentViewModel = new VGKViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка навигации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToNotificationLists()
        {
            try
            {
                CurrentViewModel = new NotificationListsViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка навигации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
