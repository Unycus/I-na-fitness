using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace FitnessApp
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                using (var context = new Data.FitnessContext())
                {
                    // Проверка существования пользователя
                    if (context.Users.Any(u => u.Username == txtUsername.Text))
                    {
                        ShowError("Пользователь с таким логином уже существует");
                        return;
                    }

                    // Создание нового пользователя
                    var newUser = new Models.User
                    {
                        Username = txtUsername.Text,
                        Password = txtPassword.Password,
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text,
                        FullName = txtFullName.Text,
                        Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text,
                        BirthDate = dpBirthDate.SelectedDate,
                        RegistrationDate = DateTime.Now,
                        IsActive = true
                    };

                    context.Users.Add(newUser);
                    context.SaveChanges();

                    MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти в систему.",
                                  "Успешная регистрация",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);

                    this.Close();
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                ShowError($"Ошибка сохранения данных: {GetInnerExceptionMessage(ex)}");
                System.Diagnostics.Debug.WriteLine($"DbUpdateException: {ex.Message}");
            }
            catch (System.Data.Entity.Core.EntityException ex)
            {
                ShowError("Ошибка подключения к базе данных.");
                System.Diagnostics.Debug.WriteLine($"EntityException: {ex.Message}");
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при регистрации: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
            }
        }

        private string GetInnerExceptionMessage(Exception ex)
        {
            if (ex.InnerException != null)
                return GetInnerExceptionMessage(ex.InnerException);
            return ex.Message;
        }

        private bool ValidateInput()
        {
            // Проверка логина
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || txtUsername.Text.Length < 3)
            {
                ShowError("Логин должен содержать минимум 3 символа");
                return false;
            }

            // Проверка пароля
            if (string.IsNullOrWhiteSpace(txtPassword.Password) || txtPassword.Password.Length < 6)
            {
                ShowError("Пароль должен содержать минимум 6 символов");
                return false;
            }

            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                ShowError("Пароли не совпадают");
                return false;
            }

            // Проверка ФИО
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                ShowError("Введите ФИО");
                return false;
            }

            // Проверка email (если указан)
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !IsValidEmail(txtEmail.Text))
            {
                ShowError("Введите корректный email адрес");
                return false;
            }

            HideError();
            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            txtError.Visibility = Visibility.Collapsed;
        }
    }
}