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
    public class StaffViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private ObservableCollection<Staff> _staffMembers;
        private Staff _selectedStaff;
        private string _searchText;
        private ObservableCollection<NotificationList> _notificationLists;

        public ObservableCollection<Staff> StaffMembers
        {
            get => _staffMembers;
            set
            {
                _staffMembers = value;
                OnPropertyChanged();
            }
        }

        public Staff SelectedStaff
        {
            get => _selectedStaff;
            set
            {
                _selectedStaff = value;
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
                SearchStaff();
            }
        }

        public ObservableCollection<NotificationList> NotificationLists
        {
            get => _notificationLists;
            set
            {
                _notificationLists = value;
                OnPropertyChanged();
            }
        }

        // Команды
        public ICommand AddStaffCommand { get; }
        public ICommand EditStaffCommand { get; }
        public ICommand DeleteStaffCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleActiveCommand { get; }

        public StaffViewModel()
        {
            _unitOfWork = new UnitOfWork(new DatabaseContext());

            AddStaffCommand = new RelayCommand(_ => AddStaff());
            EditStaffCommand = new RelayCommand(_ => EditStaff(), _ => SelectedStaff != null);
            DeleteStaffCommand = new RelayCommand(_ => DeleteStaff(), _ => SelectedStaff != null);
            RefreshCommand = new RelayCommand(_ => LoadStaff());
            ToggleActiveCommand = new RelayCommand(_ => ToggleActive(), _ => SelectedStaff != null);

            LoadNotificationLists();
            LoadStaff();
        }

        private void LoadStaff()
        {
            try
            {
                var staff = _unitOfWork.Staff.GetAllWithDetails();
                StaffMembers = new ObservableCollection<Staff>(staff);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadNotificationLists()
        {
            try
            {
                var lists = _unitOfWork.NotificationLists.GetAll();
                NotificationLists = new ObservableCollection<NotificationList>(lists);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки списков оповещения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchStaff()
        {
            try
            {
                var staff = _unitOfWork.Staff.SearchStaff(SearchText);
                StaffMembers = new ObservableCollection<Staff>(staff);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddStaff()
        {
            try
            {
                var newStaff = new Staff
                {
                    Name = "Новый",
                    Surname = "Сотрудник",
                    Position = "Инженер",
                    IsActive = true
                };

                _unitOfWork.Staff.Add(newStaff);
                _unitOfWork.SaveChanges();
                LoadStaff();

                MessageBox.Show("Сотрудник добавлен успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditStaff()
        {
            if (SelectedStaff == null) return;

            try
            {
                _unitOfWork.Staff.Update(SelectedStaff);
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

        private void DeleteStaff()
        {
            if (SelectedStaff == null) return;

            var result = MessageBox.Show(
                $"Удалить сотрудника {SelectedStaff.Surname} {SelectedStaff.Name}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _unitOfWork.Staff.Remove(SelectedStaff);
                    _unitOfWork.SaveChanges();
                    LoadStaff();

                    MessageBox.Show("Сотрудник удалён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ToggleActive()
        {
            if (SelectedStaff == null) return;

            try
            {
                SelectedStaff.IsActive = !SelectedStaff.IsActive;
                _unitOfWork.Staff.Update(SelectedStaff);
                _unitOfWork.SaveChanges();
                LoadStaff();

                string status = SelectedStaff.IsActive ? "активирован" : "деактивирован";
                MessageBox.Show($"Сотрудник {status}!", "Успех",
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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
