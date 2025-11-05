using System.Data.Entity;
using FitnessApp.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Migrations;

namespace FitnessApp.Data
{
    public class FitnessContext : DbContext
    {
        public FitnessContext() : base("name=FitnessConnectionString")
        {
            // Отключаем автоматические миграции и инициализатор для отладки
            Database.SetInitializer<FitnessContext>(null);

            // Включаем логирование SQL запросов для отладки
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<UserService> UserServices { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Убираем множественное число названий таблиц
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Явно указываем названия таблиц
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Service>().ToTable("Services");
            modelBuilder.Entity<Schedule>().ToTable("Schedules");
            modelBuilder.Entity<UserService>().ToTable("UserServices");
            modelBuilder.Entity<Booking>().ToTable("Bookings");

            base.OnModelCreating(modelBuilder);
        }
    }
}