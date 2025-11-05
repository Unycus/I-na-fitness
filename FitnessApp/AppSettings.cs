using FitnessApp.Models;

namespace FitnessApp
{
    public static class AppSettings
    {
        public static User CurrentUser { get; set; }
        public static bool IsLoggedIn => CurrentUser != null;

        // Настройки приложения
        public static string Theme { get; set; } = "Light";
        public static bool NotificationsEnabled { get; set; } = true;
        public static string FontSize { get; set; } = "Medium";
    }
}