using FitnessApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace FitnessApp.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        private User _currentUser;
        private ObservableCollection<UserService> _userServices;
        private ObservableCollection<Schedule> _availableSchedules;

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UserService> UserServices
        {
            get => _userServices;
            set
            {
                _userServices = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Schedule> AvailableSchedules
        {
            get => _availableSchedules;
            set
            {
                _availableSchedules = value;
                OnPropertyChanged();
            }
        }

        public UserViewModel()
        {
            try
            {
                LoadUserData();
                LoadUserServices();
                LoadAvailableSchedules();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadUserData()
        {
            if (AppSettings.CurrentUser != null)
            {
                try
                {
                    using (var context = new Data.FitnessContext())
                    {
                        CurrentUser = context.Users
                            .AsNoTracking()
                            .FirstOrDefault(u => u.UserId == AppSettings.CurrentUser.UserId);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных пользователя: {ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void LoadUserServices()
        {
            if (AppSettings.CurrentUser == null)
            {
                UserServices = new ObservableCollection<UserService>();
                return;
            }

            try
            {
                using (var context = new Data.FitnessContext())
                {
                    var userServices = context.UserServices
                        .Where(us => us.UserId == AppSettings.CurrentUser.UserId)
                        .ToList();

                    var serviceIds = userServices.Select(us => us.ServiceId).ToList();
                    var services = context.Services
                        .Where(s => serviceIds.Contains(s.ServiceId))
                        .ToDictionary(s => s.ServiceId);

                    var result = userServices.Select(us => new UserService
                    {
                        UserServiceId = us.UserServiceId,
                        UserId = us.UserId,
                        ServiceId = us.ServiceId,
                        PurchaseDate = us.PurchaseDate,
                        ExpiryDate = us.ExpiryDate,
                        IsActive = us.IsActive,
                        Service = services.ContainsKey(us.ServiceId) ? services[us.ServiceId] : null
                    }).ToList();

                    UserServices = new ObservableCollection<UserService>(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки услуг: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                UserServices = new ObservableCollection<UserService>();
            }
        }

        public void LoadAvailableSchedules()
        {
            try
            {
                using (var context = new Data.FitnessContext())
                {
                    var schedules = context.Schedules
                        .Where(s => s.StartTime > DateTime.Now)
                        .ToList();

                    var serviceIds = schedules.Select(s => s.ServiceId).ToList();
                    var services = context.Services
                        .Where(s => serviceIds.Contains(s.ServiceId))
                        .ToDictionary(s => s.ServiceId);

                    var result = schedules.Select(s => new Schedule
                    {
                        ScheduleId = s.ScheduleId,
                        ServiceId = s.ServiceId,
                        TrainerName = s.TrainerName,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        MaxParticipants = s.MaxParticipants,
                        CurrentParticipants = s.CurrentParticipants,
                        Service = services.ContainsKey(s.ServiceId) ? services[s.ServiceId] : null,
                        CanBook = CheckIfCanBook(s.ScheduleId)
                    }).OrderBy(s => s.StartTime).ToList();

                    AvailableSchedules = new ObservableCollection<Schedule>(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки расписания: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                AvailableSchedules = new ObservableCollection<Schedule>();
            }
        }

        public bool BookSchedule(int scheduleId)
        {
            if (AppSettings.CurrentUser == null) return false;

            try
            {
                using (var context = new Data.FitnessContext())
                {
                    var schedule = context.Schedules.Find(scheduleId);
                    if (schedule == null || schedule.CurrentParticipants >= schedule.MaxParticipants)
                        return false;

                    var existingBooking = context.Bookings
                        .FirstOrDefault(b => b.UserId == AppSettings.CurrentUser.UserId &&
                                           b.ScheduleId == scheduleId &&
                                           b.Status == "Active");

                    if (existingBooking != null)
                        return false;

                    var booking = new Booking
                    {
                        UserId = AppSettings.CurrentUser.UserId,
                        ScheduleId = scheduleId,
                        BookingDate = DateTime.Now
                    };

                    schedule.CurrentParticipants++;
                    context.Bookings.Add(booking);
                    context.SaveChanges();

                    LoadAvailableSchedules();
                    LoadUserServices();

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при записи на тренировку: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool UpdateUserProfile()
        {
            try
            {
                using (var context = new Data.FitnessContext())
                {
                    var user = context.Users.Find(CurrentUser.UserId);
                    if (user != null)
                    {
                        user.FullName = CurrentUser.FullName;
                        user.Email = CurrentUser.Email;
                        user.Phone = CurrentUser.Phone;
                        user.BirthDate = CurrentUser.BirthDate;

                        context.SaveChanges();
                        AppSettings.CurrentUser = user;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении профиля: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }

        public bool CancelBooking(int bookingId)
        {
            try
            {
                using (var context = new Data.FitnessContext())
                {
                    var booking = context.Bookings.Find(bookingId);
                    if (booking == null) return false;

                    var schedule = context.Schedules.Find(booking.ScheduleId);
                    if (schedule != null)
                    {
                        schedule.CurrentParticipants--;
                    }

                    booking.Status = "Cancelled";
                    context.SaveChanges();

                    // Обновляем данные
                    LoadAvailableSchedules();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отмене записи: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private bool CheckIfCanBook(int scheduleId)
        {
            if (AppSettings.CurrentUser == null) return false;

            try
            {
                using (var context = new Data.FitnessContext())
                {
                    var schedule = context.Schedules.Find(scheduleId);
                    if (schedule == null || schedule.StartTime <= DateTime.Now ||
                        schedule.CurrentParticipants >= schedule.MaxParticipants)
                        return false;

                    var existingBooking = context.Bookings
                        .FirstOrDefault(b => b.UserId == AppSettings.CurrentUser.UserId &&
                                           b.ScheduleId == scheduleId &&
                                           b.Status == "Active");

                    return existingBooking == null;
                }
            }
            catch
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}