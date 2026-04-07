using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using FlowerShop.ApplicationData;

namespace FlowerShop.Pages
{
    public partial class AdminPanel : Page
    {
        public AdminPanel()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            string adminName = AppSession.CurrentModelUser?.FullName ?? "Администратор";
            TbAdminInfo.Text = $"Вы вошли как: {adminName} (роль: {AppSession.CurrentRole})";

            LvTables.ItemsSource = new List<string>
            {
                "1. Users",
                "2. Roles",
                "3. Flowers",
                "4. Categories",
                "5. Bouquets",
                "6. Orders",
                "7. OrderItems",
                "8. Payments",
                "9. DeliveryMethods",
                "10. Suppliers",
                "11. InventoryMovements",
                "12. Discounts",
                "13. Carts",
                "14. CartItems",
                "15. Reviews"
            };
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.Navigate(new PageTask());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            AppSession.CurrentModelUser = null;
            AppSession.CurrentUser = null;
            AppSession.CurrentRole = "user";
            AppFrame.frmMain.Navigate(new Auth());
        }
    }
}
