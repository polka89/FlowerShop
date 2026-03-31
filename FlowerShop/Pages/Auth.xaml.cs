using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FlowerShop.ApplicationData;

namespace FlowerShop.Pages
{
    public partial class Auth : Page
    {
        public Auth()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text.Trim();
            string password = PbPassword.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль.");
                return;
            }

            var user = AppConnect.model01.customers
                .ToList()
                .FirstOrDefault(c => TryParseAuth(c.address, out string customerLogin, out string customerPassword, out _) &&
                                     customerLogin.Equals(login, StringComparison.OrdinalIgnoreCase) &&
                                     customerPassword == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.");
                return;
            }

            TryParseAuth(user.address, out _, out _, out string role);
            AppSession.CurrentUser = user;
            AppSession.CurrentRole = string.IsNullOrWhiteSpace(role) ? "user" : role.ToLower();
            user.last_login = DateTime.Now;
            AppConnect.model01.SaveChanges();

            AppFrame.frmMain.Navigate(new PageTask());
        }

        private void BtnGoRegister_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.Navigate(new Reg());
        }

        private bool TryParseAuth(string addressData, out string login, out string password, out string role)
        {
            login = string.Empty;
            password = string.Empty;
            role = "user";

            if (string.IsNullOrWhiteSpace(addressData))
            {
                return false;
            }

            string[] parts = addressData.Split('|');
            if (parts.Length < 2)
            {
                return false;
            }

            login = parts[0];
            password = parts[1];
            if (parts.Length > 2)
            {
                role = parts[2];
            }

            return true;
        }
    }
}
