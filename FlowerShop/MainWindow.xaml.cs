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
            frmMain.Navigate(new Auth());
        }
    }
}
