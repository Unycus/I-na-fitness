using System;
using System.Data.Entity;
using FitnessApp.Models;
using System.Linq;

namespace FitnessApp.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            try
            {
                using (var context = new FitnessContext())
                {
                    // Создаем базу данных, если она не существует
                    if (!context.Database.Exists())
                    {
                        context.Database.Create();
                        System.Diagnostics.Debug.WriteLine("База данных создана");
                    }

                    // Создаем таблицы по одной
                    CreateTablesIfNotExist(context);

                    // Заполняем начальными данными
                    SeedData(context);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new Exception($"Ошибка инициализации базы данных: {ex.Message}", ex);
            }
        }

        private static void CreateTablesIfNotExist(FitnessContext context)
        {
            try
            {
                // Проверяем и создаем таблицы в правильном порядке (с учетом зависимостей)
                CreateUsersTable(context);
                CreateServicesTable(context);
                CreateSchedulesTable(context);
                CreateUserServicesTable(context);
                CreateBookingsTable(context);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка создания таблиц: {ex.Message}", ex);
            }
        }

        private static void CreateUsersTable(FitnessContext context)
        {
            var tableExists = context.Database.SqlQuery<int?>(
                "SELECT OBJECT_ID('Users', 'U')"
            ).FirstOrDefault();

            if (tableExists == null)
            {
                context.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [Users] (
                        [UserId] INT IDENTITY (1, 1) NOT NULL,
                        [Username] NVARCHAR (50) NOT NULL,
                        [Password] NVARCHAR (255) NOT NULL,
                        [Email] NVARCHAR (100) NULL,
                        [FullName] NVARCHAR (100) NOT NULL,
                        [Phone] NVARCHAR (20) NULL,
                        [BirthDate] DATE NULL,
                        [RegistrationDate] DATETIME NOT NULL,
                        [IsActive] BIT NOT NULL,
                        CONSTRAINT [PK_Users] PRIMARY KEY ([UserId]),
                        CONSTRAINT [UK_Users_Username] UNIQUE ([Username])
                    )");
                System.Diagnostics.Debug.WriteLine("Таблица Users создана");
            }
        }

        private static void CreateServicesTable(FitnessContext context)
        {
            var tableExists = context.Database.SqlQuery<int?>(
                "SELECT OBJECT_ID('Services', 'U')"
            ).FirstOrDefault();

            if (tableExists == null)
            {
                context.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [Services] (
                        [ServiceId] INT IDENTITY (1, 1) NOT NULL,
                        [Name] NVARCHAR (100) NOT NULL,
                        [Description] NVARCHAR (500) NULL,
                        [Price] DECIMAL (10, 2) NOT NULL,
                        [Duration] INT NOT NULL,
                        [Category] NVARCHAR (50) NULL,
                        [ImageUrl] NVARCHAR (255) NULL,
                        CONSTRAINT [PK_Services] PRIMARY KEY ([ServiceId])
                    )");
                System.Diagnostics.Debug.WriteLine("Таблица Services создана");
            }
        }

        private static void CreateSchedulesTable(FitnessContext context)
        {
            var tableExists = context.Database.SqlQuery<int?>(
                "SELECT OBJECT_ID('Schedules', 'U')"
            ).FirstOrDefault();

            if (tableExists == null)
            {
                context.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [Schedules] (
                        [ScheduleId] INT IDENTITY (1, 1) NOT NULL,
                        [ServiceId] INT NOT NULL,
                        [TrainerName] NVARCHAR (100) NOT NULL,
                        [StartTime] DATETIME NOT NULL,
                        [EndTime] DATETIME NOT NULL,
                        [MaxParticipants] INT NOT NULL,
                        [CurrentParticipants] INT NOT NULL DEFAULT 0,
                        CONSTRAINT [PK_Schedules] PRIMARY KEY ([ScheduleId]),
                        CONSTRAINT [FK_Schedules_Services] FOREIGN KEY ([ServiceId]) REFERENCES [Services]([ServiceId])
                    )");
                System.Diagnostics.Debug.WriteLine("Таблица Schedules создана");
            }
        }

        private static void CreateUserServicesTable(FitnessContext context)
        {
            var tableExists = context.Database.SqlQuery<int?>(
                "SELECT OBJECT_ID('UserServices', 'U')"
            ).FirstOrDefault();

            if (tableExists == null)
            {
                context.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [UserServices] (
                        [UserServiceId] INT IDENTITY (1, 1) NOT NULL,
                        [UserId] INT NOT NULL,
                        [ServiceId] INT NOT NULL,
                        [PurchaseDate] DATETIME NOT NULL,
                        [ExpiryDate] DATETIME NULL,
                        [IsActive] BIT NOT NULL DEFAULT 1,
                        CONSTRAINT [PK_UserServices] PRIMARY KEY ([UserServiceId]),
                        CONSTRAINT [FK_UserServices_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]),
                        CONSTRAINT [FK_UserServices_Services] FOREIGN KEY ([ServiceId]) REFERENCES [Services]([ServiceId])
                    )");
                System.Diagnostics.Debug.WriteLine("Таблица UserServices создана");
            }
        }

        private static void CreateBookingsTable(FitnessContext context)
        {
            var tableExists = context.Database.SqlQuery<int?>(
                "SELECT OBJECT_ID('Bookings', 'U')"
            ).FirstOrDefault();

            if (tableExists == null)
            {
                context.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [Bookings] (
                        [BookingId] INT IDENTITY (1, 1) NOT NULL,
                        [UserId] INT NOT NULL,
                        [ScheduleId] INT NOT NULL,
                        [BookingDate] DATETIME NOT NULL,
                        [Status] NVARCHAR (20) NOT NULL DEFAULT 'Active',
                        CONSTRAINT [PK_Bookings] PRIMARY KEY ([BookingId]),
                        CONSTRAINT [FK_Bookings_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]),
                        CONSTRAINT [FK_Bookings_Schedules] FOREIGN KEY ([ScheduleId]) REFERENCES [Schedules]([ScheduleId])
                    )");
                System.Diagnostics.Debug.WriteLine("Таблица Bookings создана");
            }
        }

        private static void SeedData(FitnessContext context)
        {
            // Добавляем услуги, если их нет
            if (!context.Services.Any())
            {
                var services = new[]
                {
                    new Service { Name = "Йога для начинающих", Description = "Основы йоги для новичков", Price = 500, Duration = 60, Category = "Йога" },
                    new Service { Name = "Кардио-тренировка", Description = "Интенсивная кардио нагрузка", Price = 600, Duration = 45, Category = "Кардио" },
                    new Service { Name = "Силовой тренинг", Description = "Тренировка с весами", Price = 700, Duration = 60, Category = "Силовые" },
                    new Service { Name = "Плавание", Description = "Занятия в бассейне", Price = 400, Duration = 45, Category = "Бассейн" }
                };

                context.Services.AddRange(services);
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine("Тестовые услуги добавлены");
            }

            // Добавляем администратора, если его нет
            if (!context.Users.Any(u => u.Username == "admin"))
            {
                var admin = new User
                {
                    Username = "admin",
                    Password = "admin123",
                    Email = "admin@fitness.ru",
                    FullName = "Администратор Системы",
                    Phone = "+7 (999) 123-45-67",
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                };

                context.Users.Add(admin);
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine("Администратор добавлен");
            }

            // Добавляем тестового пользователя, если его нет
            if (!context.Users.Any(u => u.Username == "test"))
            {
                var testUser = new User
                {
                    Username = "test",
                    Password = "test123",
                    Email = "test@fitness.ru",
                    FullName = "Тестовый Пользователь",
                    Phone = "+7 (999) 999-99-99",
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                };

                context.Users.Add(testUser);
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine("Тестовый пользователь добавлен");
            }

            // Добавляем расписание, если его нет
            if (!context.Schedules.Any())
            {
                var yogaService = context.Services.First(s => s.Category == "Йога");
                var cardioService = context.Services.First(s => s.Category == "Кардио");

                var schedules = new[]
                {
                    new Schedule
                    {
                        ServiceId = yogaService.ServiceId,
                        TrainerName = "Анна Петрова",
                        StartTime = DateTime.Today.AddDays(1).AddHours(10),
                        EndTime = DateTime.Today.AddDays(1).AddHours(11),
                        MaxParticipants = 15,
                        CurrentParticipants = 5
                    },
                    new Schedule
                    {
                        ServiceId = cardioService.ServiceId,
                        TrainerName = "Иван Сидоров",
                        StartTime = DateTime.Today.AddDays(1).AddHours(18),
                        EndTime = DateTime.Today.AddDays(1).AddHours(18).AddMinutes(45),
                        MaxParticipants = 10,
                        CurrentParticipants = 3
                    }
                };

                context.Schedules.AddRange(schedules);
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine("Тестовое расписание добавлено");
            }

            // Добавляем тестовые покупки услуг
            if (!context.UserServices.Any())
            {
                var testUser = context.Users.First(u => u.Username == "test");
                var yogaService = context.Services.First(s => s.Category == "Йога");

                var userService = new UserService
                {
                    UserId = testUser.UserId,
                    ServiceId = yogaService.ServiceId,
                    PurchaseDate = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddMonths(1),
                    IsActive = true
                };

                context.UserServices.Add(userService);
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine("Тестовая покупка услуги добавлена");
            }
        }

        public static void RecreateDatabase()
        {
            try
            {
                using (var context = new FitnessContext())
                {
                    // Удаляем существующую базу данных
                    if (context.Database.Exists())
                    {
                        context.Database.Delete();
                        System.Diagnostics.Debug.WriteLine("Существующая база данных удалена");
                    }

                    // Создаем новую базу данных
                    context.Database.Create();
                    System.Diagnostics.Debug.WriteLine("Новая база данных создана");

                    // Создаем таблицы
                    CreateTablesIfNotExist(context);

                    // Заполняем тестовыми данными
                    SeedTestData(context);

                    System.Diagnostics.Debug.WriteLine("База данных успешно пересоздана");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка пересоздания базы данных: {ex.Message}");
                throw new Exception($"Не удалось пересоздать базу данных: {ex.Message}", ex);
            }
        }

        public static void SeedTestData(FitnessContext context)
        {
            try
            {
                // Добавляем тестовые услуги
                var services = new[]
                {
            new Service { Name = "Йога для начинающих", Description = "Основы йоги для новичков", Price = 500, Duration = 60, Category = "Йога" },
            new Service { Name = "Кардио-тренировка", Description = "Интенсивная кардио нагрузка", Price = 600, Duration = 45, Category = "Кардио" },
            new Service { Name = "Силовой тренинг", Description = "Тренировка с весами", Price = 700, Duration = 60, Category = "Силовые" },
            new Service { Name = "Плавание", Description = "Занятия в бассейне", Price = 400, Duration = 45, Category = "Бассейн" }
        };

                context.Services.AddRange(services);
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine("Тестовые услуги добавлены");

                // Добавляем администратора
                var admin = new User
                {
                    Username = "admin",
                    Password = "admin123",
                    Email = "admin@fitness.ru",
                    FullName = "Администратор Системы",
                    Phone = "+7 (999) 123-45-67",
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                };

                context.Users.Add(admin);
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine("Администратор добавлен");

                // Добавляем тестового пользователя
                var testUser = new User
                {
                    Username = "test",
                    Password = "test123",
                    Email = "test@fitness.ru",
                    FullName = "Тестовый Пользователь",
                    Phone = "+7 (999) 999-99-99",
                    RegistrationDate = DateTime.Now,
                    IsActive = true
                };

                context.Users.Add(testUser);
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine("Тестовый пользователь добавлен");

                // Добавляем тестовое расписание
                var yogaService = context.Services.First(s => s.Category == "Йога");
                var cardioService = context.Services.First(s => s.Category == "Кардио");

                var schedules = new[]
                {
            new Schedule
            {
                ServiceId = yogaService.ServiceId,
                TrainerName = "Анна Петрова",
                StartTime = DateTime.Today.AddDays(1).AddHours(10),
                EndTime = DateTime.Today.AddDays(1).AddHours(11),
                MaxParticipants = 15,
                CurrentParticipants = 5
            },
            new Schedule
            {
                ServiceId = cardioService.ServiceId,
                TrainerName = "Иван Сидоров",
                StartTime = DateTime.Today.AddDays(1).AddHours(18),
                EndTime = DateTime.Today.AddDays(1).AddHours(18).AddMinutes(45),
                MaxParticipants = 10,
                CurrentParticipants = 3
            }
        };

                context.Schedules.AddRange(schedules);
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine("Тестовое расписание добавлено");

                // Добавляем тестовые покупки услуг
                var userService = new UserService
                {
                    UserId = testUser.UserId,
                    ServiceId = yogaService.ServiceId,
                    PurchaseDate = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddMonths(1),
                    IsActive = true
                };

                context.UserServices.Add(userService);
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine("Тестовая покупка услуги добавлена");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка заполнения тестовыми данными: {ex.Message}", ex);
            }
        }
    }
}