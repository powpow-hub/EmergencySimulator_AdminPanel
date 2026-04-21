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
    public class TrainingResultsViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        private ObservableCollection<TrainingResult> _trainingResults;
        private TrainingResult _selectedResult;
        private string _searchText;
        private DateTime? _startDate;
        private DateTime? _endDate;

        public ObservableCollection<TrainingResult> TrainingResults
        {
            get => _trainingResults;
            set
            {
                _trainingResults = value;
                OnPropertyChanged();
            }
        }

        public TrainingResult SelectedResult
        {
            get => _selectedResult;
            set
            {
                _selectedResult = value;
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
                SearchResults();
            }
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
                SearchResults();
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
                SearchResults();
            }
        }

        // Команды
        public ICommand ViewDetailsCommand { get; }
        public ICommand DeleteResultCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public TrainingResultsViewModel()
        {
            _unitOfWork = new UnitOfWork(new DatabaseContext());

            ViewDetailsCommand = new RelayCommand(_ => ViewDetails(), _ => SelectedResult != null);
            DeleteResultCommand = new RelayCommand(_ => DeleteResult(), _ => SelectedResult != null);
            RefreshCommand = new RelayCommand(_ => LoadResults());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());

            LoadResults();
        }

        private void LoadResults()
        {
            try
            {
                var results = _unitOfWork.TrainingResults.GetAllWithDetails();
                TrainingResults = new ObservableCollection<TrainingResult>(results);
            }
            catch (Exception ex)
            {
                string fullError = ex.Message;
                var inner = ex.InnerException;
                while (inner != null)
                {
                    fullError += $"\n\nInner: {inner.Message}";
                    inner = inner.InnerException;
                }
                MessageBox.Show(fullError, "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchResults()
        {
            try
            {
                var results = _unitOfWork.TrainingResults.FilterResults(SearchText, StartDate, EndDate);
                TrainingResults = new ObservableCollection<TrainingResult>(results);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFilters()
        {
            SearchText = string.Empty;
            StartDate = null;
            EndDate = null;
            LoadResults();
        }

        private void ViewDetails()
        {
            if (SelectedResult == null) return;

            try
            {
                var details = $"Пользователь: {SelectedResult.User.Surname} {SelectedResult.User.Name}\n" +
                             $"Начало: {SelectedResult.SessionStart:dd.MM.yyyy HH:mm}\n" +
                             $"Окончание: {(SelectedResult.SessionEnd.HasValue ? SelectedResult.SessionEnd.Value.ToString("dd.MM.yyyy HH:mm") : "Не завершено")}\n" +
                             $"Баллы: {SelectedResult.TotalScore}\n" +
                             $"Критические ошибки: {SelectedResult.CriticalErrorsCount}\n" +
                             $"Значительные ошибки: {SelectedResult.SignificantErrorsCount}\n" +
                             $"Нарушения времени: {SelectedResult.TimeViolationsCount}\n" +
                             $"Статус: {SelectedResult.Status}\n" +
                             $"Отчёт: {(string.IsNullOrEmpty(SelectedResult.PDFReportPath) ? "Не создан" : SelectedResult.PDFReportPath)}";

                MessageBox.Show(details, "Детали тренировки",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отображения деталей: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteResult()
        {
            if (SelectedResult == null) return;

            var result = MessageBox.Show(
                $"Удалить результат тренировки пользователя {SelectedResult.User.Surname} {SelectedResult.User.Name}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _unitOfWork.TrainingResults.Remove(SelectedResult);
                    _unitOfWork.SaveChanges();
                    LoadResults();

                    MessageBox.Show("Результат удалён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
