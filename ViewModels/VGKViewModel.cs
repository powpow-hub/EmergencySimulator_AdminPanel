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
    public class VGKViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private ObservableCollection<VGKMember> _vgkMembers;
        private VGKMember _selectedMember;
        private ObservableCollection<Staff> _availableStaff;
        private string _searchText;
        private bool _isLoading;
        private string _filterRole;
        private bool _showOnDutyOnly;

        public ObservableCollection<VGKMember> VGKMembers
        {
            get => _vgkMembers;
            set
            {
                _vgkMembers = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalMembers));
                OnPropertyChanged(nameof(OnDutyCount));
                OnPropertyChanged(nameof(CommanderCount));
            }
        }

        public VGKMember SelectedMember
        {
            get => _selectedMember;
            set
            {
                _selectedMember = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsMemberSelected));
            }
        }

        public ObservableCollection<Staff> AvailableStaff
        {
            get => _availableStaff;
            set
            {
                _availableStaff = value;
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
                ApplyFilters();
            }
        }

        public string FilterRole
        {
            get => _filterRole;
            set
            {
                _filterRole = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public bool ShowOnDutyOnly
        {
            get => _showOnDutyOnly;
            set
            {
                _showOnDutyOnly = value;
                OnPropertyChanged();
                ApplyFilters();
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

        public bool IsMemberSelected => SelectedMember != null;
        public int TotalMembers => VGKMembers?.Count ?? 0;
        public int OnDutyCount => VGKMembers?.Count(m => m.IsOnDuty) ?? 0;
        public int CommanderCount => VGKMembers?.Count(m => m.IsCommander) ?? 0;

        public ObservableCollection<string> AvailableRoles { get; }

        // Команды
        public ICommand AddMemberCommand { get; }
        public ICommand EditMemberCommand { get; }
        public ICommand DeleteMemberCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleDutyCommand { get; }
        public ICommand SetAsCommanderCommand { get; }
        public ICommand ViewMemberDetailsCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public VGKViewModel()
        {
            _unitOfWork = new UnitOfWork(new DatabaseContext());

            AvailableRoles = new ObservableCollection<string>
            {
                "Командир",
                "Заместитель командира",
                "Спасатель",
                "Медик",
                "Связист",
                "Техник",
                "Водитель"
            };

            AddMemberCommand = new RelayCommand(_ => AddMember());
            EditMemberCommand = new RelayCommand(_ => EditMember(), _ => SelectedMember != null);
            DeleteMemberCommand = new RelayCommand(_ => DeleteMember(), _ => SelectedMember != null);
            RefreshCommand = new RelayCommand(_ => LoadMembers());
            ToggleDutyCommand = new RelayCommand(_ => ToggleDuty(), _ => SelectedMember != null);
            SetAsCommanderCommand = new RelayCommand(_ => SetAsCommander(), _ => SelectedMember != null);
            ViewMemberDetailsCommand = new RelayCommand(_ => ViewMemberDetails(), _ => SelectedMember != null);
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());

            LoadAvailableStaff();
            LoadMembers();
        }

        private void LoadMembers()
        {
            try
            {
                IsLoading = true;
                var members = _unitOfWork.VGKMembers.GetAllWithStaff();
                VGKMembers = new ObservableCollection<VGKMember>(members);
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

        private void LoadAvailableStaff()
        {
            try
            {
                var staff = _unitOfWork.Staff.GetActiveStaff();
                AvailableStaff = new ObservableCollection<Staff>(staff);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки персонала: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                IsLoading = true;
                var members = _unitOfWork.VGKMembers.FilterMembers(SearchText, FilterRole, ShowOnDutyOnly);
                VGKMembers = new ObservableCollection<VGKMember>(members);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка применения фильтров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            FilterRole = null;
            ShowOnDutyOnly = false;
            LoadMembers();
        }

        private void AddMember()
        {
            try
            {
                // Открываем окно выбора сотрудника
                var selectStaffWindow = new Views.SelectStaffWindow();

                if (selectStaffWindow.ShowDialog() == true && selectStaffWindow.SelectedStaff != null)
                {
                    var selectedStaff = selectStaffWindow.SelectedStaff;

                    var newMember = new VGKMember
                    {
                        StaffID = selectedStaff.StaffID,
                        Role = "Спасатель",
                        IsCommander = false,
                        IsOnDuty = true
                    };

                    _unitOfWork.VGKMembers.Add(newMember);
                    _unitOfWork.SaveChanges();
                    LoadMembers();

                    MessageBox.Show(
                        $"Сотрудник {selectedStaff.Surname} {selectedStaff.Name} добавлен в состав ВГК!",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditMember()
        {
            if (SelectedMember == null) return;

            try
            {
                _unitOfWork.VGKMembers.Update(SelectedMember);
                _unitOfWork.SaveChanges();

                MessageBox.Show("Изменения сохранены!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                LoadMembers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteMember()
        {
            if (SelectedMember == null) return;

            var message = SelectedMember.IsCommander
                ? $"ВНИМАНИЕ! Вы удаляете командира ВГК!\n\n" +
                  $"Удалить {SelectedMember.Staff.Surname} {SelectedMember.Staff.Name} из состава ВГК?"
                : $"Удалить {SelectedMember.Staff.Surname} {SelectedMember.Staff.Name} из состава ВГК?";

            var result = MessageBox.Show(
                message,
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                SelectedMember.IsCommander ? MessageBoxImage.Warning : MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _unitOfWork.VGKMembers.Remove(SelectedMember);
                    _unitOfWork.SaveChanges();
                    LoadMembers();

                    MessageBox.Show("Член ВГК удалён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ToggleDuty()
        {
            if (SelectedMember == null) return;

            try
            {
                SelectedMember.IsOnDuty = !SelectedMember.IsOnDuty;
                _unitOfWork.VGKMembers.Update(SelectedMember);
                _unitOfWork.SaveChanges();

                LoadMembers();

                string status = SelectedMember.IsOnDuty ? "НА ДЕЖУРСТВЕ" : "НЕ НА ДЕЖУРСТВЕ";
                MessageBox.Show(
                    $"{SelectedMember.Staff.Surname} {SelectedMember.Staff.Name}\n" +
                    $"Статус изменён: {status}",
                    "Статус обновлён",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка изменения статуса: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetAsCommander()
        {
            if (SelectedMember == null) return;

            var currentCommander = _unitOfWork.VGKMembers.GetCommander();

            string message;
            if (SelectedMember.IsCommander)
            {
                message = $"Снять статус командира с {SelectedMember.Staff.Surname} {SelectedMember.Staff.Name}?";
            }
            else if (currentCommander != null && currentCommander.MemberID != SelectedMember.MemberID)
            {
                message = $"Текущий командир: {currentCommander.Staff.Surname} {currentCommander.Staff.Name}\n\n" +
                         $"Назначить командиром {SelectedMember.Staff.Surname} {SelectedMember.Staff.Name}?\n\n" +
                         $"Текущий командир будет снят с должности.";
            }
            else
            {
                message = $"Назначить командиром {SelectedMember.Staff.Surname} {SelectedMember.Staff.Name}?";
            }

            var result = MessageBox.Show(message, "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (!SelectedMember.IsCommander && currentCommander != null &&
                        currentCommander.MemberID != SelectedMember.MemberID)
                    {
                        currentCommander.IsCommander = false;
                        currentCommander.Role = "Спасатель";
                        _unitOfWork.VGKMembers.Update(currentCommander);
                    }

                    SelectedMember.IsCommander = !SelectedMember.IsCommander;
                    if (SelectedMember.IsCommander)
                    {
                        SelectedMember.Role = "Командир";
                    }

                    _unitOfWork.VGKMembers.Update(SelectedMember);
                    _unitOfWork.SaveChanges();
                    LoadMembers();

                    MessageBox.Show("Статус командира обновлён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка назначения: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ViewMemberDetails()
        {
            if (SelectedMember == null) return;

            try
            {
                var staff = SelectedMember.Staff;
                var details = $"ИНФОРМАЦИЯ О ЧЛЕНЕ ВГК\n" +
                             $"═══════════════════════════════\n\n" +
                             $"ФИО: {staff.Surname} {staff.Name} {staff.MiddleName}\n" +
                             $"Должность: {staff.Position}\n" +
                             $"Телефон: {staff.Phone ?? "Не указан"}\n\n" +
                             $"ДАННЫЕ ВГК:\n" +
                             $"───────────────────────────────\n" +
                             $"Роль в ВГК: {SelectedMember.Role}\n" +
                             $"Командир: {(SelectedMember.IsCommander ? "ДА" : "Нет")}\n" +
                             $"На дежурстве: {(SelectedMember.IsOnDuty ? "ДА" : "Нет")}\n" +
                             $"Статус сотрудника: {(staff.IsActive ? "Активен" : "Неактивен")}\n" +
                             $"Список оповещения: {staff.NotificationList?.ListName ?? "Не назначен"}";

                MessageBox.Show(details, "Детали члена ВГК",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка получения данных: {ex.Message}", "Ошибка",
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
