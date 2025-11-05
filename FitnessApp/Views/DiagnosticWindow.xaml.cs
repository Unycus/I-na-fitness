using System;
using System.Linq;
using System.Text;
using System.Windows;

namespace FitnessApp
{
    public partial class DiagnosticWindow : Window
    {
        public DiagnosticWindow()
        {
            InitializeComponent();
        }

        private void BtnCheckDb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new Data.FitnessContext())
                {
                    bool exists = context.Database.Exists();
                    txtDbStatus.Text = $"Статус: {(exists ? "✅ База данных существует" : "❌ База данных не существует")}";

                    if (exists)
                    {
                        var connection = context.Database.Connection;
                        txtDbDetails.Text = $"База данных: {connection.Database}\nСервер: {connection.DataSource}";
                    }
                    else
                    {
                        txtDbDetails.Text = "База данных не найдена. Используйте кнопку 'Пересоздать базу данных'.";
                    }
                }
            }
            catch (Exception ex)
            {
                txtDbStatus.Text = "Статус: ❌ Ошибка подключения";
                txtDbDetails.Text = $"Ошибка: {GetFullErrorMessage(ex)}";
            }
        }

        private void BtnCheckTables_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sb = new StringBuilder();
                using (var context = new Data.FitnessContext())
                {
                    if (!context.Database.Exists())
                    {
                        txtTablesStatus.Text = "Статус таблиц: ❌ База данных не существует";
                        return;
                    }

                    // Проверяем существование таблиц
                    var tableNames = new[] { "Users", "Services", "Schedules", "UserServices", "Bookings" };
                    foreach (var tableName in tableNames)
                    {
                        try
                        {
                            var tableExists = context.Database.SqlQuery<int?>(
                                $"SELECT OBJECT_ID('{tableName}', 'U')"
                            ).FirstOrDefault();

                            if (tableExists != null)
                            {
                                var count = context.Database.SqlQuery<int>($"SELECT COUNT(*) FROM {tableName}").First();
                                sb.AppendLine($"✅ Таблица {tableName}: {count} записей");
                            }
                            else
                            {
                                sb.AppendLine($"❌ Таблица {tableName}: НЕ СУЩЕСТВУЕТ");
                            }
                        }
                        catch (Exception ex)
                        {
                            sb.AppendLine($"❌ Таблица {tableName}: ошибка - {ex.Message}");
                        }
                    }
                }

                txtTablesStatus.Text = "Статус таблиц: Проверено";
                txtTablesDetails.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                txtTablesStatus.Text = "Статус таблиц: ❌ Ошибка проверки";
                txtTablesDetails.Text = $"Ошибка: {GetFullErrorMessage(ex)}";
            }
        }

        private void BtnRecreateDb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Вы уверены, что хотите пересоздать базу данных? Все данные будут потеряны!",
                                           "Подтверждение",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    txtRecreateStatus.Text = "Создание базы данных...";

                    Data.DatabaseInitializer.RecreateDatabase();

                    txtRecreateStatus.Text = "✅ База данных успешно пересоздана и заполнена тестовыми данными\n\n" +
                                           "Тестовые пользователи:\n" +
                                           "• Логин: admin, Пароль: admin123\n" +
                                           "• Логин: test, Пароль: test123";

                    // Обновляем статус таблиц
                    BtnCheckTables_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                txtRecreateStatus.Text = $"❌ Ошибка при пересоздании базы данных: {GetFullErrorMessage(ex)}";
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string GetFullErrorMessage(Exception ex)
        {
            string message = ex.Message;
            Exception inner = ex.InnerException;
            while (inner != null)
            {
                message += $"\n-> {inner.Message}";
                inner = inner.InnerException;
            }
            return message;
        }
    }
}