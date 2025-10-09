using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;


namespace Ifitapp
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            LoadServices();
        }

        private void LoadServices()
        {
            // Временные данные для демонстрации
            var services = new List<Service>
            {
                new Service { ServiceId = 1, ServiceName = "Персональные тренировки", ImagePath = "/Images/personal_training.jpg", ImageColor = "#FFE57373" },
                new Service { ServiceId = 2, ServiceName = "Групповые занятия", ImagePath = "/Images/group_classes.jpg", ImageColor = "#FF81C784" },
                new Service { ServiceId = 3, ServiceName = "Йога и пилатес", ImagePath = "/Images/yoga.jpg", ImageColor = "#FF64B5F6" },
                new Service { ServiceId = 4, ServiceName = "Бассейн", ImagePath = "/Images/pool.jpg", ImageColor = "#FF4DB6AC" },
                new Service { ServiceId = 5, ServiceName = "Кардио зона", ImagePath = "/Images/cardio.jpg", ImageColor = "#FFF48FB1" },
                new Service { ServiceId = 6, ServiceName = "Силовые тренировки", ImagePath = "/Images/strength.jpg", ImageColor = "#FF9575CD" },
                new Service { ServiceId = 7, ServiceName = "Спа и массаж", ImagePath = "/Images/spa.jpg", ImageColor = "#FF4FC3F7" },
                new Service { ServiceId = 8, ServiceName = "Фитнес-бар", ImagePath = "/Images/bar.jpg", ImageColor = "#FFF06292" }
            };

            ServiceItemsControl.ItemsSource = services;
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is int serviceId)
            {
                // Здесь будет навигация на страницу с подробной информацией
                MessageBox.Show($"Подробная информация об услуге ID: {serviceId}",
                              "Подробности",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
            }
        }
    }

    // Класс для хранения данных об услугах
    public class Service
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string ImageColor { get; set; } = string.Empty;
    }
}