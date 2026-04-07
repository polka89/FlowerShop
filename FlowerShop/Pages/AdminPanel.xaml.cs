using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using FlowerShop.ApplicationData;

namespace FlowerShop.Pages
{
    public partial class AdminPanel : Page
    {
        private readonly Model1Container _context;

        public ObservableCollection<categories> Categories { get; private set; }
        public ObservableCollection<flowers> Flowers { get; private set; }
        public ObservableCollection<customers> Customers { get; private set; }
        public ObservableCollection<orders> Orders { get; private set; }
        public ObservableCollection<order_items> OrderItems { get; private set; }
        public ObservableCollection<order_statuses> OrderStatuses { get; private set; }
        public ObservableCollection<payments> Payments { get; private set; }
        public ObservableCollection<payment_methods> PaymentMethods { get; private set; }
        public ObservableCollection<deliveries> Deliveries { get; private set; }
        public ObservableCollection<employees> Employees { get; private set; }
        public ObservableCollection<suppliers> Suppliers { get; private set; }
        public ObservableCollection<inventory_batches> InventoryBatches { get; private set; }
        public ObservableCollection<bouquet_templates> BouquetTemplates { get; private set; }
        public ObservableCollection<discounts> Discounts { get; private set; }
        public ObservableCollection<reviews> Reviews { get; private set; }

        public AdminPanel()
        {
            InitializeComponent();
            _context = AppConnect.model01;
            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            Categories = _context.categories.Local;
            Flowers = _context.flowers.Local;
            Customers = _context.customers.Local;
            Orders = _context.orders.Local;
            OrderItems = _context.order_items.Local;
            OrderStatuses = _context.order_statuses.Local;
            Payments = _context.payments.Local;
            PaymentMethods = _context.payment_methods.Local;
            Deliveries = _context.deliveries.Local;
            Employees = _context.employees.Local;
            Suppliers = _context.suppliers.Local;
            InventoryBatches = _context.inventory_batches.Local;
            BouquetTemplates = _context.bouquet_templates.Local;
            Discounts = _context.discounts.Local;
            Reviews = _context.reviews.Local;

            _context.categories.Load();
            _context.flowers.Load();
            _context.customers.Load();
            _context.orders.Load();
            _context.order_items.Load();
            _context.order_statuses.Load();
            _context.payments.Load();
            _context.payment_methods.Load();
            _context.deliveries.Load();
            _context.employees.Load();
            _context.suppliers.Load();
            _context.inventory_batches.Load();
            _context.bouquet_templates.Load();
            _context.discounts.Load();
            _context.reviews.Load();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            _context.SaveChanges();
            MessageBox.Show("Изменения сохранены.");
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.Navigate(new PageTask());
        }
    }
}
