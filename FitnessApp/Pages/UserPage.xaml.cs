using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FitnessApp.ViewModels;
using System.Data.Entity;

namespace FitnessApp.Pages
{
    public partial class UserPage : Page
    {
        private UserViewModel _viewModel;

        public UserPage()
        {
            try
            {
                InitializeComponent();

                if (!AppSettings.IsLoggedIn)
                {
                    MessageBox.Show("Пожалуйста, войдите в систему",
                                  "Требуется авторизация",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }

                _viewModel = new UserViewModel();
                this.DataContext = _viewModel;

                LoadUserData();
                UpdateVisibility();
                LoadMyBookings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке личного кабинета: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUserData()
        {
            if (_viewModel?.CurrentUser != null)
            {
                txtFullName.Text = _viewModel.CurrentUser.FullName;
                txtEmail.Text = _viewModel.CurrentUser.Email ?? "";
                txtPhone.Text = _viewModel.CurrentUser.Phone ?? "";
                dpBirthDate.SelectedDate = _viewModel.CurrentUser.BirthDate;
            }
        }

        private void LoadMyBookings()
        {
            try
            {
                using (var context = new Data.FitnessContext())
                {
                    if (AppSettings.CurrentUser != null)
                    {
                        var myBookings = context.Bookings
                            .Where(b => b.UserId == AppSettings.CurrentUser.UserId && b.Status == "Active")
                            .Include("Schedule.Service")
                            .OrderBy(b => b.Schedule.StartTime)
                            .ToList();

                        dgMyBookings.ItemsSource = myBookings;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки записей: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateVisibility()
        {
            try
            {
                if (dgSchedule.ItemsSource != null)
                {
                    txtNoSchedules.Visibility = dgSchedule.Items.Count == 0 ?
                        Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    txtNoSchedules.Visibility = Visibility.Visible;
                }

                if (dgUserServices.ItemsSource != null)
                {
                    txtNoServices.Visibility = dgUserServices.Items.Count == 0 ?
                        Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    txtNoServices.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                txtNoSchedules.Visibility = Visibility.Visible;
                txtNoServices.Visibility = Visibility.Visible;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel?.CurrentUser == null) return;

            try
            {
                _viewModel.CurrentUser.FullName = txtFullName.Text;
                _viewModel.CurrentUser.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text;
                _viewModel.CurrentUser.Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text;
                _viewModel.CurrentUser.BirthDate = dpBirthDate.SelectedDate;

                if (_viewModel.UpdateUserProfile())
                {
                    MessageBox.Show("Данные успешно сохранены!", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ошибка при сохранении данных", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null &&
                int.TryParse(button.Tag.ToString(), out int scheduleId))
            {
                try
                {
                    if (_viewModel.BookSchedule(scheduleId))
                    {
                        MessageBox.Show("Вы успешно записаны на тренировку!", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        UpdateVisibility();
                        LoadMyBookings(); // Обновляем список записей
                    }
                    else
                    {
                        MessageBox.Show("Не удалось записаться на тренировку. Возможно, нет свободных мест или вы уже записаны.",
                                      "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при записи: {ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelBookingButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null &&
                int.TryParse(button.Tag.ToString(), out int bookingId))
            {
                try
                {
                    var result = MessageBox.Show("Вы уверены, что хотите отменить запись?",
                                               "Подтверждение отмены",
                                               MessageBoxButton.YesNo,
                                               MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (_viewModel.CancelBooking(bookingId))
                        {
                            MessageBox.Show("Запись успешно отменена!", "Успех",
                                          MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadMyBookings();
                            _viewModel.LoadAvailableSchedules(); // Обновляем расписание
                        }
                        else
                        {
                            MessageBox.Show("Не удалось отменить запись", "Ошибка",
                                          MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отмене записи: {ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnBuyServices_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу услуг для покупки
            NavigationService?.Navigate(new MainPage());
        }

        private void CmbScheduleFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Фильтрация расписания будет добавлена позже
            // Пока оставляем пустым или добавляем базовую логику
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null && e.Source is TabControl)
            {
                try
                {
                    _viewModel.LoadUserServices();
                    _viewModel.LoadAvailableSchedules();
                    UpdateVisibility();

                    // При переключении на вкладку записей обновляем список записей
                    var tabControl = sender as TabControl;
                    if (tabControl != null && tabControl.SelectedIndex == 1) // Вкладка "Запись на тренировки"
                    {
                        LoadMyBookings();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}