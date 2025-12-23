using EmergencySimulator.AdminPanel.Data;
using EmergencySimulator.AdminPanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EmergencySimulator.AdminPanel.Views
{
    public partial class ManageListMembersWindow : Window
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _listId;
        private List<Staff> _allStaff;
        private List<Staff> _availableStaff;
        private List<Staff> _listMembers;

        public ManageListMembersWindow(int listId, string listName)
        {
            InitializeComponent();

            _unitOfWork = new UnitOfWork(new DatabaseContext());
            _listId = listId;

            ListNameText.Text = listName;

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Загружаем всех активных сотрудников
                _allStaff = _unitOfWork.Staff.GetActiveStaff().ToList();

                // Загружаем текущих членов списка
                _listMembers = _unitOfWork.Staff.GetStaffByNotificationList(_listId).ToList();

                // Определяем доступных сотрудников (те, которых нет в списке)
                _availableStaff = _allStaff
                    .Where(s => !_listMembers.Any(m => m.StaffID == s.StaffID))
                    .ToList();

                UpdateLists();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void UpdateLists()
        {
            // Обновляем списки
            AvailableStaffListBox.ItemsSource = null;
            AvailableStaffListBox.ItemsSource = _availableStaff;

            ListMembersListBox.ItemsSource = null;
            ListMembersListBox.ItemsSource = _listMembers;

            // Обновляем счетчики
            AvailableCountText.Text = $"Всего: {_availableStaff.Count}";
            MembersCountText.Text = $"В списке: {_listMembers.Count}";
        }

        private void AddToListButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = AvailableStaffListBox.SelectedItems.Cast<Staff>().ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show(
                    "Пожалуйста, выберите сотрудников для добавления в список",
                    "Выбор сотрудников",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            foreach (var staff in selected)
            {
                _availableStaff.Remove(staff);
                _listMembers.Add(staff);
            }

            UpdateLists();
        }

        private void AddAllToListButton_Click(object sender, RoutedEventArgs e)
        {
            if (_availableStaff.Count == 0)
            {
                MessageBox.Show(
                    "Нет доступных сотрудников для добавления",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Добавить всех доступных сотрудников ({_availableStaff.Count}) в список?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _listMembers.AddRange(_availableStaff);
                _availableStaff.Clear();
                UpdateLists();
            }
        }

        private void RemoveFromListButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = ListMembersListBox.SelectedItems.Cast<Staff>().ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show(
                    "Пожалуйста, выберите сотрудников для удаления из списка",
                    "Выбор сотрудников",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            foreach (var staff in selected)
            {
                _listMembers.Remove(staff);
                _availableStaff.Add(staff);
            }

            UpdateLists();
        }

        private void RemoveAllFromListButton_Click(object sender, RoutedEventArgs e)
        {
            if (_listMembers.Count == 0)
            {
                MessageBox.Show(
                    "Список уже пуст",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить всех сотрудников ({_listMembers.Count}) из списка?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _availableStaff.AddRange(_listMembers);
                _listMembers.Clear();
                UpdateLists();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Обновляем ListID для всех членов списка
                foreach (var staff in _listMembers)
                {
                    staff.ListID = _listId;
                    _unitOfWork.Staff.Update(staff);
                }

                // Убираем ListID у тех, кого удалили из списка
                foreach (var staff in _availableStaff)
                {
                    // Проверяем, был ли этот сотрудник в списке ранее
                    var originalStaff = _unitOfWork.Staff.GetById(staff.StaffID);
                    if (originalStaff != null && originalStaff.ListID == _listId)
                    {
                        staff.ListID = null;
                        _unitOfWork.Staff.Update(staff);
                    }
                }

                _unitOfWork.SaveChanges();

                MessageBox.Show(
                    $"Состав списка успешно обновлен!\n\n" +
                    $"Членов в списке: {_listMembers.Count}",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка сохранения: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SearchAvailableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchAvailableTextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                AvailableStaffListBox.ItemsSource = _availableStaff;
            }
            else
            {
                var filtered = _availableStaff.Where(s =>
                    s.Surname.ToLower().Contains(searchText) ||
                    s.Name.ToLower().Contains(searchText) ||
                    (s.MiddleName != null && s.MiddleName.ToLower().Contains(searchText)) ||
                    s.Position.ToLower().Contains(searchText) ||
                    (s.Phone != null && s.Phone.Contains(searchText))
                ).ToList();

                AvailableStaffListBox.ItemsSource = filtered;
            }
        }

        private void SearchMembersTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchMembersTextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                ListMembersListBox.ItemsSource = _listMembers;
            }
            else
            {
                var filtered = _listMembers.Where(s =>
                    s.Surname.ToLower().Contains(searchText) ||
                    s.Name.ToLower().Contains(searchText) ||
                    (s.MiddleName != null && s.MiddleName.ToLower().Contains(searchText)) ||
                    s.Position.ToLower().Contains(searchText) ||
                    (s.Phone != null && s.Phone.Contains(searchText))
                ).ToList();

                ListMembersListBox.ItemsSource = filtered;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var hasChanges = CheckForChanges();

            if (hasChanges)
            {
                var result = MessageBox.Show(
                    "У вас есть несохраненные изменения. Закрыть без сохранения?",
                    "Несохраненные изменения",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            DialogResult = false;
            Close();
        }

        private bool CheckForChanges()
        {
            // Проверяем, изменился ли состав списка
            var originalMembers = _unitOfWork.Staff.GetStaffByNotificationList(_listId).ToList();

            if (originalMembers.Count != _listMembers.Count)
                return true;

            foreach (var member in _listMembers)
            {
                if (!originalMembers.Any(m => m.StaffID == member.StaffID))
                    return true;
            }

            return false;
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
