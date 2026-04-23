using FlowerShop.ApplicationData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace FlowerShop.Pages
{
    public partial class Reg : Page
    {
        public Reg()
        {
            InitializeComponent();
        }

        private void Input_TextChanged(object sender, EventArgs e)
        {
            ValidateForm();
        }

        private void PasswordBoxes_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateForm();
        }

        private void ValidateForm()
        {
            bool isValid = true;
            string errorMessage = "";

            if (string.IsNullOrWhiteSpace(TbUserName.Text))
            {
                isValid = false;
                errorMessage += "• Введите имя пользователя\n";
            }

            if (DpBirthDate.SelectedDate == null)
            {
                isValid = false;
                errorMessage += "• Выберите дату рождения\n";
            }
            else
            {
                DateTime birthDate = DpBirthDate.SelectedDate.Value;
                int age = DateTime.Now.Year - birthDate.Year;
                if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
                    age--;

                if (age < 16)
                {
                    isValid = false;
                    errorMessage += "• Возраст должен быть не менее 16 лет\n";
                }
            }

            if (string.IsNullOrWhiteSpace(TbExperience.Text))
            {
                isValid = false;
                errorMessage += "• Введите стаж работы\n";
            }
            else
            {
                if (!int.TryParse(TbExperience.Text, out int experience) || experience < 0)
                {
                    isValid = false;
                    errorMessage += "• Стаж должен быть положительным числом\n";
                }
            }

            if (string.IsNullOrWhiteSpace(TbLogin.Text))
            {
                isValid = false;
                errorMessage += "• Введите логин\n";
            }
            else if (TbLogin.Text.Length < 3)
            {
                isValid = false;
                errorMessage += "• Логин должен содержать минимум 3 символа\n";
            }
            else
            {
                if (UserService.IsLoginExists(TbLogin.Text))
                {
                    isValid = false;
                    errorMessage += "• Пользователь с таким логином уже существует\n";
                }
            }

            string password = PbPassword.Password;
            string confirmPassword = PbPasswordConfirm.Password;

            if (string.IsNullOrWhiteSpace(password))
            {
                isValid = false;
                errorMessage += "• Введите пароль\n";
            }
            else if (password.Length < 4)
            {
                isValid = false;
                errorMessage += "• Пароль должен содержать минимум 4 символа\n";
            }

            if (password != confirmPassword)
            {
                isValid = false;
                errorMessage += "• Пароли не совпадают\n";
            }

            if (string.IsNullOrWhiteSpace(TbEmail.Text))
            {
                isValid = false;
                errorMessage += "• Введите email\n";
            }
            else if (!IsValidEmail(TbEmail.Text))
            {
                isValid = false;
                errorMessage += "• Введите корректный email\n";
            }

            if (string.IsNullOrWhiteSpace(TbPhone.Text))
            {
                isValid = false;
                errorMessage += "• Введите номер телефона\n";
            }
            else if (!IsValidPhone(TbPhone.Text))
            {
                isValid = false;
                errorMessage += "• Введите корректный номер телефона (11 цифр)\n";
            }

            BtnRegister.IsEnabled = isValid;
            TbValidation.Text = errorMessage;
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

        private bool IsValidPhone(string phone)
        {
            string digits = new string(phone.Where(char.IsDigit).ToArray());
            return digits.Length == 11;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UserService.IsLoginExists(TbLogin.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newUser = new UserJson
                {
                    FullName = TbUserName.Text,
                    Login = TbLogin.Text,
                    Password = PbPassword.Password,
                    RoleId = 3,
                    BirthDate = DpBirthDate.SelectedDate?.ToString("yyyy-MM-dd"),
                    Experience = TbExperience.Text,
                    Email = TbEmail.Text,
                    Phone = TbPhone.Text
                };

                UserService.SaveUser(newUser);

                MessageBox.Show("Регистрация успешно завершена!\nТеперь вы можете войти в систему.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService?.Navigate(new Auth());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.Navigate(new Auth());
        }
    }

    // Класс для хранения пользователя в JSON
    public class UserJson
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string BirthDate { get; set; }
        public string Experience { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CreatedAt { get; set; }
    }

    // ==================== СЕРВИС ДЛЯ РАБОТЫ С ПОЛЬЗОВАТЕЛЯМИ ====================
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