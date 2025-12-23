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
    public class PLAViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private ObservableCollection<PLAData> _plaData;
        private PLAData _selectedPLA;
        private string _searchText;

        public ObservableCollection<PLAData> PLADataList
        {
            get => _plaData;
            set
            {
                _plaData = value;
                OnPropertyChanged();
            }
        }

        public PLAData SelectedPLA
        {
            get => _selectedPLA;
            set
            {
                _selectedPLA = value;
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
                SearchPLA();
            }
        }

        // Команды
        public ICommand AddPLACommand { get; }
        public ICommand EditPLACommand { get; }
        public ICommand DeletePLACommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleCriticalCommand { get; }

        public PLAViewModel()
        {
            _unitOfWork = new UnitOfWork(new DatabaseContext());

            AddPLACommand = new RelayCommand(_ => AddPLA());
            EditPLACommand = new RelayCommand(_ => EditPLA(), _ => SelectedPLA != null);
            DeletePLACommand = new RelayCommand(_ => DeletePLA(), _ => SelectedPLA != null);
            RefreshCommand = new RelayCommand(_ => LoadPLA());
            ToggleCriticalCommand = new RelayCommand(_ => ToggleCritical());

            LoadPLA();
        }

        private void LoadPLA()
        {
            try
            {
                var data = _unitOfWork.PLAData.GetAll()
                    .OrderBy(p => p.PositionNumber)
                    .ThenBy(p => p.StepOrder)
                    .ToList();
                PLADataList = new ObservableCollection<PLAData>(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchPLA()
        {
            try
            {
                var filtered = _unitOfWork.PLAData.SearchPLA(SearchText);
                PLADataList = new ObservableCollection<PLAData>(filtered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddPLA()
        {
            try
            {
                var newPLA = new PLAData
                {
                    PositionNumber = "№1",
                    PositionName = "Новая позиция",
                    ActionStep = "Описание действия",
                    StepOrder = 1,
                    TimeNormative = 120,
                    IsCritical = false
                };

                _unitOfWork.PLAData.Add(newPLA);
                _unitOfWork.SaveChanges();
                LoadPLA();

                MessageBox.Show("Запись ПЛА добавлена успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditPLA()
        {
            if (SelectedPLA == null) return;

            try
            {
                _unitOfWork.PLAData.Update(SelectedPLA);
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

        private void DeletePLA()
        {
            if (SelectedPLA == null) return;

            var result = MessageBox.Show(
                $"Удалить запись ПЛА: {SelectedPLA.PositionName} - {SelectedPLA.ActionStep}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _unitOfWork.PLAData.Remove(SelectedPLA);
                    _unitOfWork.SaveChanges();
                    LoadPLA();

                    MessageBox.Show("Запись удалена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ToggleCritical()
        {
            if (SelectedPLA == null) return;

            try
            {
                SelectedPLA.IsCritical = !SelectedPLA.IsCritical;
                _unitOfWork.PLAData.Update(SelectedPLA);
                _unitOfWork.SaveChanges();

                string status = SelectedPLA.IsCritical ? "критическим" : "обычным";
                string icon = SelectedPLA.IsCritical ? "⚠️" : "✅";

                MessageBox.Show(
                    $"{icon} Действие помечено как {status}!\n\n" +
                    $"Позиция: {SelectedPLA.PositionNumber}\n" +
                    $"Действие: {SelectedPLA.ActionStep}",
                    "Статус изменен",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Обновляем отображение
                LoadPLA();
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

