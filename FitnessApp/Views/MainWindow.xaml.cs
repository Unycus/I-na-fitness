using System.Windows;
using System.Windows.Controls;
using FitnessApp.Pages;

namespace FitnessApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadUserData();
            NavigateToMainPage();
        }

        private void LoadUserData()
        {
            if (AppSettings.CurrentUser != null)
            {
                txtWelcome.Text = $"Добро пожаловать, {AppSettings.CurrentUser.FullName}!";
            }
        }

        private void NavigateToMainPage()
        {
            MainFrame.Navigate(new MainPage());
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                try
                {
                    switch (button.Tag.ToString())
                    {
                        case "Services":
                            MainFrame.Navigate(new MainPage());
                            break;
                        case "User":
                            if (AppSettings.IsLoggedIn)
                                MainFrame.Navigate(new UserPage());
                            else
                                MessageBox.Show("Пожалуйста, войдите в систему");
                            break;
                        case "Settings":
                            MainFrame.Navigate(new SettingsPage());
                            break;
                        case "Diagnostic":
                            MainFrame.Navigate(new DiagnosticPage());
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка навигации: {ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            AppSettings.CurrentUser = null;

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void BtnDiagnostic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DiagnosticWindow diagnosticWindow = new DiagnosticWindow();
                diagnosticWindow.Owner = this;
                diagnosticWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия диагностики: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}