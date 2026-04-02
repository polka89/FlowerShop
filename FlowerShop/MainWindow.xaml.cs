using System;
using System.Windows;
using FlowerShop.ApplicationData;
using FlowerShop.Pages;

namespace FlowerShop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppFrame.frmMain = frmMain;

            try
            {
                DbBootstrapper.EnsureExtendedSchema();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации БД: {ex.Message}");
            }

            frmMain.Navigate(new Auth());
        }
    }
}
