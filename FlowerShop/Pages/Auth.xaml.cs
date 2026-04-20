using System;
using System.Windows;
using System.Windows.Controls;
using FlowerShop.ApplicationData;

namespace FlowerShop.Pages
{
    public partial class Auth : Page
    {
        private ShopModel _shop;

        public Auth()
        {
            InitializeComponent();
            _shop = new ShopModel();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text.Trim();
            string password = PbPassword.Password;

            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Введите логин!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _shop.FindUser(login, password);

            if (user != null)
            {
                // ПЕРЕДАЕМ ПОЛЬЗОВАТЕЛЯ В PageTask
                NavigationService?.Navigate(new PageTask(user));
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGoRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Reg());
        }
    }
}
