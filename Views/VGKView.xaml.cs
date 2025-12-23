using System.Windows.Controls;
using EmergencySimulator.AdminPanel.ViewModels;

namespace EmergencySimulator.AdminPanel.Views
{
    /// <summary>
    /// Логика взаимодействия для VGKView.xaml
    /// Представление для управления составом ВГК (Военизированная горноспасательная команда)
    /// 
    /// Функциональность:
    /// - Просмотр всех членов ВГК
    /// - Добавление/удаление членов команды
    /// - Назначение командира
    /// - Управление статусом дежурства
    /// - Фильтрация по ролям и статусу
    /// - Просмотр детальной информации о члене ВГК
    /// </summary>
    public partial class VGKView : UserControl
    {
        public VGKView()
        {
            InitializeComponent();

            // Автоматическая привязка ViewModel
            DataContext = new VGKViewModel();
        }
    }
}
