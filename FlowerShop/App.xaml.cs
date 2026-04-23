using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using FlowerShop.Pages;

namespace FlowerShop
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем администратора при первом запуске
            CreateAdminIfNotExists();
        }

        private void CreateAdminIfNotExists()
        {
            try
            {
                string usersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");

                // Создаем файл если не существует
                if (!File.Exists(usersFilePath))
                {
                    File.WriteAllText(usersFilePath, "[]");
                }

                // Загружаем пользователей
                string json = File.ReadAllText(usersFilePath);
                var users = JsonSerializer.Deserialize<List<UserJson>>(json) ?? new List<UserJson>();

                // Проверяем, существует ли администратор
                bool adminExists = users.Any(u => u.Login == "admin");

                if (!adminExists)
                {
                    // Создаем администратора
                    var admin = new UserJson
                    {
                        Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1,
                        FullName = "Администратор",
                        Login = "admin",
                        Password = "admin",
                        RoleId = 1,
                        BirthDate = "1990-01-01",
                        Experience = "5",
                        Email = "admin@flowwow.com",
                        Phone = "79991234567",
                        CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    users.Add(admin);

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    json = JsonSerializer.Serialize(users, options);
                    File.WriteAllText(usersFilePath, json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании администратора: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}