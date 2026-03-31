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
            ValidateForm();
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
            TbValidation.Text = string.Empty;

            bool allFilled = !string.IsNullOrWhiteSpace(TbUserName.Text)
                             && DpBirthDate.SelectedDate.HasValue
                             && !string.IsNullOrWhiteSpace(TbExperience.Text)
                             && !string.IsNullOrWhiteSpace(TbLogin.Text)
                             && !string.IsNullOrWhiteSpace(PbPassword.Password)
                             && !string.IsNullOrWhiteSpace(PbPasswordConfirm.Password)
                             && !string.IsNullOrWhiteSpace(TbEmail.Text)
                             && !string.IsNullOrWhiteSpace(TbPhone.Text);

            bool passwordsMatch = PbPassword.Password == PbPasswordConfirm.Password;
            BtnRegister.IsEnabled = allFilled && passwordsMatch;

            if (!passwordsMatch && !string.IsNullOrWhiteSpace(PbPasswordConfirm.Password))
            {
                TbValidation.Text = "Пароли не совпадают.";
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text.Trim();
            string email = TbEmail.Text.Trim();
            string phone = TbPhone.Text.Trim();

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Введите корректную почту.");
                return;
            }

            if (!Regex.IsMatch(phone, @"^[0-9\+\-\(\)\s]{7,20}$"))
            {
                MessageBox.Show("Введите корректный телефон.");
                return;
            }

            bool loginExists = AppConnect.model01.customers
                .ToList()
                .Any(c => !string.IsNullOrWhiteSpace(c.address) && c.address.Split('|')[0].Equals(login, StringComparison.OrdinalIgnoreCase));

            if (loginExists)
            {
                MessageBox.Show("Логин уже существует.");
                return;
            }

            var newCustomer = new customers
            {
                first_name = TbUserName.Text.Trim(),
                last_name = TbExperience.Text.Trim(),
                email = email,
                phone = phone,
                registration_date = DpBirthDate.SelectedDate,
                address = $"{login}|{PbPassword.Password}|user",
                last_login = null
            };

            AppConnect.model01.customers.Add(newCustomer);
            AppConnect.model01.SaveChanges();

            MessageBox.Show("Регистрация успешно завершена.");
            AppFrame.frmMain.Navigate(new Auth());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.Navigate(new Auth());
        }
    }
}
