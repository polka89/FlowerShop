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

            // Ищем пользователя через сервис
            var userJson = UserService.FindUser(login, password);

            if (userJson != null)
            {
                // Создаем объект User для передачи в PageTask
                var appUser = new User
                {
                    Id = userJson.Id,
                    Login = userJson.Login,
                    FullName = userJson.FullName,
                    RoleId = userJson.RoleId
                };
                NavigationService?.Navigate(new PageTask(appUser));
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