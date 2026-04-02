using System;
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

            if (!AuthRepository.TryAuthorize(login, password, out customers user, out string role))
            {
                MessageBox.Show("Неверный логин или пароль.");
                return;
            }

            AppSession.CurrentUser = user;
            AppSession.CurrentRole = role.ToLower();
            user.last_login = DateTime.Now;
            AppConnect.model01.SaveChanges();

            AppFrame.frmMain.Navigate(new PageTask());
        }

        private void BtnGoRegister_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.Navigate(new Reg());
        }
    }
}
