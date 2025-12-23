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
    public partial class SelectStaffWindow : Window
    {
        private readonly IUnitOfWork _unitOfWork;
        private List<Staff> _allStaff;
        private List<Staff> _filteredStaff;

        public Staff SelectedStaff { get; private set; }

        public SelectStaffWindow()
        {
            InitializeComponent();

            _unitOfWork = new UnitOfWork(new DatabaseContext());
            LoadStaff();
        }

        private void LoadStaff()
        {
            try
            {
                // Загружаем активных сотрудников, которых еще нет в ВГК
                var vgkMemberStaffIds = _unitOfWork.VGKMembers.GetAll()
                    .Select(m => m.StaffID)
                    .ToList();

                _allStaff = _unitOfWork.Staff.GetActiveStaff()
                    .Where(s => !vgkMemberStaffIds.Contains(s.StaffID))
                    .ToList();

                _filteredStaff = _allStaff;
                UpdateList();
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

        private void UpdateList()
        {
            StaffListBox.ItemsSource = null;
            StaffListBox.ItemsSource = _filteredStaff;

            if (_filteredStaff.Count > 0)
            {
                StaffListBox.SelectedIndex = 0;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredStaff = _allStaff;
            }
            else
            {
                _filteredStaff = _allStaff.Where(s =>
                    s.Surname.ToLower().Contains(searchText) ||
                    s.Name.ToLower().Contains(searchText) ||
                    (s.MiddleName != null && s.MiddleName.ToLower().Contains(searchText)) ||
                    s.Position.ToLower().Contains(searchText) ||
                    (s.Phone != null && s.Phone.Contains(searchText))
                ).ToList();
            }

            UpdateList();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (StaffListBox.SelectedItem == null)
            {
                MessageBox.Show(
                    "Пожалуйста, выберите сотрудника из списка",
                    "Выбор сотрудника",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            SelectedStaff = (Staff)StaffListBox.SelectedItem;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void StaffListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (StaffListBox.SelectedItem != null)
            {
                SelectButton_Click(sender, e);
            }
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
