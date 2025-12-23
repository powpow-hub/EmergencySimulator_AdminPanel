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
    public class EquipmentViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private ObservableCollection<Equipment> _equipmentList;
        private Equipment _selectedEquipment;
        private string _searchText;

        public ObservableCollection<Equipment> EquipmentList
        {
            get => _equipmentList;
            set
            {
                _equipmentList = value;
                OnPropertyChanged();
            }
        }

        public Equipment SelectedEquipment
        {
            get => _selectedEquipment;
            set
            {
                _selectedEquipment = value;
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
                SearchEquipment();
            }
        }

        // Команды
        public ICommand AddEquipmentCommand { get; }
        public ICommand EditEquipmentCommand { get; }
        public ICommand DeleteEquipmentCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleAvailabilityCommand { get; }

        public EquipmentViewModel()
        {
            _unitOfWork = new UnitOfWork(new DatabaseContext());

            AddEquipmentCommand = new RelayCommand(_ => AddEquipment());
            EditEquipmentCommand = new RelayCommand(_ => EditEquipment(), _ => SelectedEquipment != null);
            DeleteEquipmentCommand = new RelayCommand(_ => DeleteEquipment(), _ => SelectedEquipment != null);
            RefreshCommand = new RelayCommand(_ => LoadEquipment());
            ToggleAvailabilityCommand = new RelayCommand(_ => ToggleAvailability(), _ => SelectedEquipment != null);

            LoadEquipment();
        }

        private void LoadEquipment()
        {
            try
            {
                var equipment = _unitOfWork.Equipment.GetAll();
                EquipmentList = new ObservableCollection<Equipment>(equipment);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchEquipment()
        {
            try
            {
                var filtered = _unitOfWork.Equipment.SearchEquipment(SearchText);
                EquipmentList = new ObservableCollection<Equipment>(filtered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddEquipment()
        {
            try
            {
                var newEquipment = new Equipment
                {
                    EquipmentName = "Новое оборудование",
                    EquipmentType = "Тип",
                    Location = "Местоположение",
                    IsAvailable = true
                };

                _unitOfWork.Equipment.Add(newEquipment);
                _unitOfWork.SaveChanges();
                LoadEquipment();

                MessageBox.Show("Оборудование добавлено успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditEquipment()
        {
            if (SelectedEquipment == null) return;

            try
            {
                _unitOfWork.Equipment.Update(SelectedEquipment);
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

        private void DeleteEquipment()
        {
            if (SelectedEquipment == null) return;

            var result = MessageBox.Show(
                $"Удалить оборудование {SelectedEquipment.EquipmentName}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _unitOfWork.Equipment.Remove(SelectedEquipment);
                    _unitOfWork.SaveChanges();
                    LoadEquipment();

                    MessageBox.Show("Оборудование удалено!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ToggleAvailability()
        {
            if (SelectedEquipment == null) return;

            try
            {
                SelectedEquipment.IsAvailable = !SelectedEquipment.IsAvailable;
                _unitOfWork.Equipment.Update(SelectedEquipment);
                _unitOfWork.SaveChanges();
                LoadEquipment();

                string status = SelectedEquipment.IsAvailable ? "доступно" : "недоступно";
                MessageBox.Show($"Оборудование теперь {status}!", "Успех",
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
