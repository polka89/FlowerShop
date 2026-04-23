using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using FlowerShop.Pages;

namespace FlowerShop.Services
{
    public static class UserService
    {
        private static string _usersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
        private static readonly object _lock = new object();

        public static List<UserJson> GetAllUsers()
        {
            lock (_lock)
            {
                if (!File.Exists(_usersFilePath))
                    return new List<UserJson>();

                string json = File.ReadAllText(_usersFilePath);
                return JsonSerializer.Deserialize<List<UserJson>>(json) ?? new List<UserJson>();
            }
        }

        public static void SaveUser(UserJson user)
        {
            lock (_lock)
            {
                var users = GetAllUsers();

                // Генерируем новый ID
                user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
                user.CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                users.Add(user);

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(users, options);
                File.WriteAllText(_usersFilePath, json);
            }
        }

        public static UserJson FindUser(string login, string password)
        {
            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.Login == login && u.Password == password);
        }

        public static bool IsLoginExists(string login)
        {
            var users = GetAllUsers();
            return users.Any(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
        }
    }
}