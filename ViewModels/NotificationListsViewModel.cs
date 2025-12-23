using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using EmergencySimulator.AdminPanel.Commands;
using EmergencySimulator.AdminPanel.Data;
using EmergencySimulator.AdminPanel.Models;

namespace EmergencySimulator.AdminPanel.ViewModels
{
    public class NotificationListsViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private ObservableCollection<NotificationList> _notificationLists;
        private NotificationList _selectedList;
        private ObservableCollection<Staff> _listMembers;

        public ObservableCollection<NotificationList> NotificationLists
        {
            get => _notificationLists;
            set
            {
                _notificationLists = value;
                OnPropertyChanged();
            }
        }

        public NotificationList SelectedList
        {
            get => _selectedList;
            set
            {
                _selectedList = value;
                OnPropertyChanged();
                LoadListMembers();
            }
        }

        public ObservableCollection<Staff> ListMembers
        {
            get => _listMembers;
            set
            {
                _listMembers = value;
                OnPropertyChanged();
            }
        }

        // Команды
        public ICommand AddListCommand { get; }
        public ICommand EditListCommand { get; }
        public ICommand DeleteListCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ManageMembersCommand { get; }

        public NotificationListsViewModel()
        {
            _unitOfWork = new UnitOfWork(new DatabaseContext());

            AddListCommand = new RelayCommand(_ => AddList());
            EditListCommand = new RelayCommand(_ => EditList(), _ => SelectedList != null);
            DeleteListCommand = new RelayCommand(_ => DeleteList(), _ => SelectedList != null);
            RefreshCommand = new RelayCommand(_ => LoadLists());
            ManageMembersCommand = new RelayCommand(_ => ManageMembers(), _ => SelectedList != null);

            LoadLists();
        }

        private void LoadLists()
        {
            try
            {
                var lists = _unitOfWork.NotificationLists.GetAll();
                NotificationLists = new ObservableCollection<NotificationList>(lists);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadListMembers()
        {
            if (SelectedList == null)
            {
                ListMembers = new ObservableCollection<Staff>();
                return;
            }

            try
            {
                var members = _unitOfWork.Staff.GetStaffByNotificationList(SelectedList.ListID);
                ListMembers = new ObservableCollection<Staff>(members);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки членов списка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddList()
        {
            try
            {
                var newList = new NotificationList
                {
                    ListName = "Новый список",
                    Description = "Описание списка",
                    Priority = 1
                };

                _unitOfWork.NotificationLists.Add(newList);
                _unitOfWork.SaveChanges();
                LoadLists();

                MessageBox.Show("Список оповещения добавлен успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditList()
        {
            if (SelectedList == null) return;

            try
            {
                _unitOfWork.NotificationLists.Update(SelectedList);
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

        private void DeleteList()
        {
            if (SelectedList == null) return;

            var result = MessageBox.Show(
                $"Удалить список оповещения '{SelectedList.ListName}'?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _unitOfWork.NotificationLists.Remove(SelectedList);
                    _unitOfWork.SaveChanges();
                    LoadLists();

                    MessageBox.Show("Список удалён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ManageMembers()
        {
            if (SelectedList == null) return;

            try
            {
                var manageMembersWindow = new Views.ManageListMembersWindow(
                    SelectedList.ListID,
                    SelectedList.ListName);

                if (manageMembersWindow.ShowDialog() == true)
                {
                    // Обновляем список членов после изменений
                    LoadListMembers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка управления составом: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
