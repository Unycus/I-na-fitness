using FitnessApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FitnessApp.Services
{
    public class DatabaseService
    {
        private readonly Data.FitnessContext _context;

        public DatabaseService()
        {
            _context = new Data.FitnessContext();
        }

        #region User Operations

        public User AuthenticateUser(string username, string password)
        {
            return _context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password && u.IsActive);
        }

        public bool UserExists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

        public User CreateUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return user;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при создании пользователя: {ex.Message}");
                return null;
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                var existingUser = _context.Users.Find(user.UserId);
                if (existingUser != null)
                {
                    _context.Entry(existingUser).CurrentValues.SetValues(user);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при обновлении пользователя: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Service Operations

        public List<Service> GetAllServices()
        {
            return _context.Services.Where(s => s.Price > 0).ToList();
        }

        public List<Service> GetServicesByCategory(string category)
        {
            return _context.Services
                .Where(s => s.Category == category && s.Price > 0)
                .ToList();
        }

        public Service GetServiceById(int serviceId)
        {
            return _context.Services.Find(serviceId);
        }

        public List<Service> SearchServices(string searchTerm)
        {
            return _context.Services
                .Where(s => s.Name.Contains(searchTerm) || s.Description.Contains(searchTerm))
                .ToList();
        }

        #endregion

        #region UserService Operations

        public UserService PurchaseService(int userId, int serviceId, DateTime? expiryDate = null)
        {
            try
            {
                var userService = new UserService
                {
                    UserId = userId,
                    ServiceId = serviceId,
                    PurchaseDate = DateTime.Now,
                    ExpiryDate = expiryDate ?? DateTime.Now.AddMonths(1), // По умолчанию на 1 месяц
                    IsActive = true
                };

                _context.UserServices.Add(userService);
                _context.SaveChanges();

                return userService;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при покупке услуги: {ex.Message}");
                return null;
            }
        }

        public List<UserService> GetUserServices(int userId)
        {
            return _context.UserServices
                .Where(us => us.UserId == userId)
                .Include(us => us.Service)
                .ToList();
        }

        public List<UserService> GetActiveUserServices(int userId)
        {
            return _context.UserServices
                .Where(us => us.UserId == userId && us.IsActive &&
                           (us.ExpiryDate == null || us.ExpiryDate > DateTime.Now))
                .Include(us => us.Service)
                .ToList();
        }

        #endregion

        #region Schedule Operations

        public List<Schedule> GetAvailableSchedules()
        {
            return _context.Schedules
                .Where(s => s.StartTime > DateTime.Now &&
                          s.CurrentParticipants < s.MaxParticipants)
                .Include(s => s.Service)
                .OrderBy(s => s.StartTime)
                .ToList();
        }

        public List<Schedule> GetSchedulesByService(int serviceId)
        {
            return _context.Schedules
                .Where(s => s.ServiceId == serviceId && s.StartTime > DateTime.Now)
                .Include(s => s.Service)
                .OrderBy(s => s.StartTime)
                .ToList();
        }

        public Schedule GetScheduleById(int scheduleId)
        {
            return _context.Schedules
                .Include(s => s.Service)
                .FirstOrDefault(s => s.ScheduleId == scheduleId);
        }

        #endregion

        #region Booking Operations

        public Booking CreateBooking(int userId, int scheduleId)
        {
            try
            {
                var schedule = _context.Schedules.Find(scheduleId);
                if (schedule == null || schedule.CurrentParticipants >= schedule.MaxParticipants)
                    return null;

                // Проверяем, не записан ли уже пользователь
                var existingBooking = _context.Bookings
                    .FirstOrDefault(b => b.UserId == userId &&
                                       b.ScheduleId == scheduleId &&
                                       b.Status == "Active");

                if (existingBooking != null)
                    return null;

                var booking = new Booking
                {
                    UserId = userId,
                    ScheduleId = scheduleId,
                    BookingDate = DateTime.Now,
                    Status = "Active"
                };

                schedule.CurrentParticipants++;

                _context.Bookings.Add(booking);
                _context.SaveChanges();

                return booking;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при создании записи: {ex.Message}");
                return null;
            }
        }

        public List<Booking> GetUserBookings(int userId)
        {
            return _context.Bookings
                .Where(b => b.UserId == userId && b.Status == "Active")
                .Include(b => b.Schedule.Service)
                .OrderBy(b => b.Schedule.StartTime)
                .ToList();
        }

        public bool CancelBooking(int bookingId)
        {
            try
            {
                var booking = _context.Bookings.Find(bookingId);
                if (booking == null) return false;

                var schedule = _context.Schedules.Find(booking.ScheduleId);
                if (schedule != null)
                {
                    schedule.CurrentParticipants--;
                }

                booking.Status = "Cancelled";
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при отмене записи: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Statistics and Reports

        public int GetTotalUsers()
        {
            return _context.Users.Count();
        }

        public int GetActiveUsers()
        {
            return _context.Users.Count(u => u.IsActive);
        }

        public decimal GetTotalRevenue()
        {
            return _context.UserServices.Sum(us => us.Service.Price);
        }

        public List<Service> GetPopularServices(int count = 5)
        {
            return _context.UserServices
                .GroupBy(us => us.ServiceId)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => g.FirstOrDefault().Service)
                .ToList();
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}