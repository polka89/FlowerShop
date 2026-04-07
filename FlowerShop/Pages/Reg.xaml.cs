using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using FlowerShop.ApplicationData;

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
                var existingUser = AppConnect.model01.Users
                    .FirstOrDefault(u => u.Login.Equals(TbLogin.Text, StringComparison.OrdinalIgnoreCase));

                if (existingUser != null)
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
                User newUser = new User
                {
                    Id = AppConnect.model01.Users.Count + 1,
                    FullName = TbUserName.Text,
                    Login = TbLogin.Text,
                    Password = PbPassword.Password,
                    RoleId = 3,
                    CreatedAt = DateTime.Now
                };

                AppConnect.model01.Users.Add(newUser);

                MessageBox.Show("Регистрация успешно завершена!\nТеперь вы можете войти в систему.",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                AppFrame.frmMain.Navigate(new Auth());
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
}