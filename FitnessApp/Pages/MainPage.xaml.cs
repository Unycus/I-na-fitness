using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FitnessApp.ViewModels;

namespace FitnessApp.Pages
{
    public partial class MainPage : Page
    {
        private MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            this.DataContext = _viewModel;
            ServicesItemsControl.ItemsSource = _viewModel.FilteredServices;

            // Устанавливаем подсказку вручную
            SetSearchPlaceholder();
        }

        private void SetSearchPlaceholder()
        {
            txtSearch.Text = "Поиск услуг...";
            txtSearch.Foreground = System.Windows.Media.Brushes.Gray;
        }

        private void ClearSearchPlaceholder()
        {
            txtSearch.Text = "";
            txtSearch.Foreground = System.Windows.Media.Brushes.Black;
        }

        private void CmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null && cmbCategory.SelectedItem != null)
            {
                var selectedItem = cmbCategory.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    _viewModel.SelectedCategory = selectedItem.Content.ToString();
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_viewModel != null && sender is TextBox textBox)
            {
                // Игнорируем текст placeholder при поиске
                if (textBox.Text != "Поиск услуг...")
                {
                    _viewModel.SearchText = textBox.Text;
                }
                else
                {
                    _viewModel.SearchText = "";
                }
            }
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text == "Поиск услуг...")
            {
                ClearSearchPlaceholder();
            }
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
            {
                SetSearchPlaceholder();
            }
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AppSettings.IsLoggedIn)
            {
                MessageBox.Show("Пожалуйста, войдите в систему для покупки услуг",
                              "Требуется авторизация",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            if (sender is Button button && button.Tag != null && int.TryParse(button.Tag.ToString(), out int serviceId))
            {
                try
                {
                    using (var context = new Data.FitnessContext())
                    {
                        var service = context.Services.Find(serviceId);
                        if (service != null)
                        {
                            var result = MessageBox.Show($"Купить услугу '{service.Name}' за {service.Price}₽?",
                                                       "Подтверждение покупки",
                                                       MessageBoxButton.YesNo,
                                                       MessageBoxImage.Question);

                            if (result == MessageBoxResult.Yes)
                            {
                                var userService = new Models.UserService
                                {
                                    UserId = AppSettings.CurrentUser.UserId,
                                    ServiceId = serviceId,
                                    PurchaseDate = DateTime.Now,
                                    ExpiryDate = DateTime.Now.AddMonths(1)
                                };

                                context.UserServices.Add(userService);
                                context.SaveChanges();

                                MessageBox.Show("Услуга успешно приобретена!", "Успех",
                                              MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при покупке услуги: {ex.Message}",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
        }
    }
}