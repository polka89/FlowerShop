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

            // Исправлено: customers -> Users
            var user = AppConnect.model01.Users
                .FirstOrDefault(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase) &&
                                     u.Password == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.");
                return;
            }

            // Устанавливаем роль на основе RoleId
            var role = AppConnect.model01.Roles.FirstOrDefault(r => r.Id == user.RoleId);
            AppSession.CurrentUser = user;
            AppSession.CurrentRole = role?.Name ?? "user";

            // Убираем SaveChanges() так как у ShopModel его нет
            // user.last_login = DateTime.Now; // Если нужно, добавьте поле LastLogin в класс User

            MessageBox.Show($"Добро пожаловать, {user.FullName}!");
            AppFrame.frmMain.Navigate(new PageTask());
        }

        private void BtnGoRegister_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.Navigate(new Reg());
        }
    }
}