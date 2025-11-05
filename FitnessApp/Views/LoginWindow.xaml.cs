using System;
using System.Linq;
using System.Windows;

namespace FitnessApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Введите логин и пароль");
                return;
            }

            try
            {
                using (var context = new Data.FitnessContext())
                {
                    // Проверяем существование базы данных
                    if (!context.Database.Exists())
                    {
                        ShowError("База данных не найдена. Используйте 'Аварийный доступ' для создания.");
                        return;
                    }

                    var user = context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
                    if (user != null && user.IsActive)
                    {
                        AppSettings.CurrentUser = user;
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        ShowError("Неверный логин или пароль");
                    }
                }
            }
            catch (System.Data.Entity.Core.EntityException ex)
            {
                ShowError("Ошибка подключения к базе данных. Используйте 'Аварийный доступ'.");
                System.Diagnostics.Debug.WriteLine($"EntityException: {ex.Message}");
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка входа: {ex.Message}");
            }
        }

        private void BtnEmergency_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Показываем прогресс
                ShowError("Создание базы данных...");

                // Создаем базу данных и заполняем тестовыми данными
                Data.DatabaseInitializer.RecreateDatabase();

                // Создаем тестового пользователя для входа
                CreateEmergencyUser();

                ShowError("✅ База данных создана! Используйте логин: admin, пароль: admin123");

                // Автоматически заполняем поля для тестового входа
                txtUsername.Text = "admin";
                txtPassword.Password = "admin123";
            }
            catch (Exception ex)
            {
                ShowError($"❌ Ошибка создания базы данных: {ex.Message}");
            }
        }

        private void CreateEmergencyUser()
        {
            try
            {
                using (var context = new Data.FitnessContext())
                {
                    // Проверяем, есть ли уже пользователь admin
                    var existingAdmin = context.Users.FirstOrDefault(u => u.Username == "admin");
                    if (existingAdmin == null)
                    {
                        var admin = new Models.User
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
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка создания тестового пользователя: {ex.Message}", ex);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем существование базы данных перед регистрацией
                using (var context = new Data.FitnessContext())
                {
                    if (!context.Database.Exists())
                    {
                        ShowError("База данных не найдена. Используйте 'Аварийный доступ' для создания.");
                        return;
                    }
                }

                RegisterWindow registerWindow = new RegisterWindow();
                registerWindow.Owner = this;
                registerWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }
    }
}