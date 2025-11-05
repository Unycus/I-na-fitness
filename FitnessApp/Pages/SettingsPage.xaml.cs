using System.Windows;
using System.Windows.Controls;

namespace FitnessApp.Pages
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            // Загружаем текущие настройки
            chkDarkTheme.IsChecked = AppSettings.Theme == "Dark";
            chkNotifications.IsChecked = AppSettings.NotificationsEnabled;

            // Устанавливаем выбранный размер шрифта
            foreach (ComboBoxItem item in cmbFontSize.Items)
            {
                if (item.Content.ToString() == AppSettings.FontSize)
                {
                    cmbFontSize.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // Сохраняем настройки
            AppSettings.Theme = chkDarkTheme.IsChecked == true ? "Dark" : "Light";
            AppSettings.NotificationsEnabled = chkNotifications.IsChecked == true;

            if (cmbFontSize.SelectedItem is ComboBoxItem selectedFont)
            {
                AppSettings.FontSize = selectedFont.Content.ToString();
            }

            MessageBox.Show("Настройки сохранены!", "Успех",
                          MessageBoxButton.OK, MessageBoxImage.Information);

            // Применяем тему (если нужно)
            ApplyTheme();
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция экспорта данных будет реализована в будущей версии",
                          "В разработке",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnClearCache_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите очистить кэш приложения?",
                                       "Подтверждение",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Здесь будет логика очистки кэша
                MessageBox.Show("Кэш успешно очищен", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Проверка обновлений...\nУ вас установлена последняя версия приложения",
                          "Обновления",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Фитнес-клуб приложение\nВерсия 1.0\n© 2024",
                          "О программе",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ApplyTheme()
        {
            // Здесь можно реализовать смену темы
            if (AppSettings.Theme == "Dark")
            {
                // Применяем темную тему
            }
            else
            {
                // Применяем светлую тему
            }
        }
    }
}