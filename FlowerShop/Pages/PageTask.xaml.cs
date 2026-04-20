using FlowerShop.ApplicationData;
using FlowerShop.Data;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using MediaColor = System.Windows.Media.Color;

namespace FlowerShop.Pages
{
    // ==================== КОНВЕРТЕРЫ ====================

    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            string fileName = value.ToString();
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string[] paths = {
                Path.Combine(basePath, "images", fileName),
                Path.Combine(basePath, fileName),
                Path.Combine(basePath, "..", "..", "images", fileName)
            };
            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        return new System.Windows.Media.Imaging.BitmapImage(new Uri(path));
                    }
                    catch { }
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AvailabilityTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAvailable)
            {
                return isAvailable ? "В наличии" : "Нет в наличии";
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AvailabilityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAvailable)
            {
                return isAvailable ? "#9C9E4A" : "#F896A3";
            }
            return "#CCBE83";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            string fileName = value.ToString();
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string[] paths = {
                Path.Combine(basePath, "Images", fileName),
                Path.Combine(basePath, fileName),
                Path.Combine(basePath, "..", "..", "Images", fileName)
            };
            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(path, UriKind.Absolute);
                        bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        return bitmap;
                    }
                    catch { }
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StockColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int stock)
            {
                if (stock <= 0) return new SolidColorBrush(Colors.Red);
                if (stock <= 5) return new SolidColorBrush(Colors.Orange);
                return new SolidColorBrush(Colors.Green);
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // ==================== ОСНОВНОЙ КЛАСС ====================

    public partial class PageTask : Page
    {
        private ShopModel _shop;
        private DatabaseHelper _dbHelper;
        private int _currentPage = 1;
        private int _itemsPerPage = 6;
        private List<FlowerProduct> _allProducts;
        private int? _selectedCategoryId = null;
        private User _currentUser;

        private List<CartItem> _cart = new List<CartItem>();
        private List<OrderInfo> _orders = new List<OrderInfo>();
        private Dictionary<int, int> _selectedQuantities = new Dictionary<int, int>();

        public class CartItem
        {
            public FlowerProduct Product { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice => (Product?.Price ?? 0) * Quantity;
        }

        public class OrderInfo
        {
            public string OrderDate { get; set; }
            public string ItemsCount { get; set; }
            public decimal TotalAmount { get; set; }
            public List<CartItem> Items { get; set; }
            public int OrderNumber { get; set; }
            public string DeliveryMethod { get; set; }
            public decimal DeliveryPrice { get; set; }
            public string DeliveryAddress { get; set; }
            public decimal FinalTotal => TotalAmount + DeliveryPrice;
        }

        public PageTask(User user = null)
        {
            InitializeComponent();

            _dbHelper = new DatabaseHelper();
            _currentUser = user;
            _shop = new ShopModel();

            LoadDataFromDatabase();
            LoadCategories();
            LoadProducts();

            if (_currentUser != null && _currentUser.RoleId == 1)
            {
                BtnAdd.Visibility = Visibility.Visible;
                BtnEdit.Visibility = Visibility.Visible;
                BtnDelete.Visibility = Visibility.Visible;
            }
            else
            {
                BtnAdd.Visibility = Visibility.Collapsed;
                BtnEdit.Visibility = Visibility.Collapsed;
                BtnDelete.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                var productsFromDb = _dbHelper.GetAllProducts();
                if (productsFromDb != null && productsFromDb.Any())
                {
                    _allProducts = productsFromDb;
                }
                else
                {
                    _allProducts = _shop.Flowers.Where(f => f.IsAvailable).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                _allProducts = _shop.Flowers.Where(f => f.IsAvailable).ToList();
            }
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _dbHelper.GetAllCategories();
                if (categories != null && categories.Any())
                {
                    CategoriesPanel.ItemsSource = categories;
                }
                else
                {
                    CategoriesPanel.ItemsSource = _shop.Categories;
                }
            }
            catch
            {
                CategoriesPanel.ItemsSource = _shop.Categories;
            }
        }

        private void LoadProducts()
        {
            var products = _allProducts;
            if (_selectedCategoryId.HasValue)
                products = products.Where(p => p.CategoryId == _selectedCategoryId.Value).ToList();

            var pagedProducts = products.Skip((_currentPage - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();
            ProductsListView.ItemsSource = pagedProducts;
        }

        private void BtnAllCategories_Click(object sender, RoutedEventArgs e)
        {
            _selectedCategoryId = null;
            _currentPage = 1;
            LoadProducts();
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                _selectedCategoryId = int.Parse(button.Tag.ToString());
                _currentPage = 1;
                LoadProducts();
            }
        }

        private void BtnCart_Click(object sender, RoutedEventArgs e)
        {
            CatalogView.Visibility = Visibility.Collapsed;
            CartView.Visibility = Visibility.Visible;
            HistoryView.Visibility = Visibility.Collapsed;
            UpdateCartDisplay();
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            CatalogView.Visibility = Visibility.Collapsed;
            CartView.Visibility = Visibility.Collapsed;
            HistoryView.Visibility = Visibility.Visible;
            UpdateHistoryDisplay();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                NavigationService?.Navigate(new Auth());
            }
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.Tag as FlowerProduct;
            if (product != null && product.Stock > 0)
            {
                int currentQty = _selectedQuantities.ContainsKey(product.Id) ? _selectedQuantities[product.Id] : 1;
                if (currentQty < product.Stock)
                {
                    currentQty++;
                    _selectedQuantities[product.Id] = currentQty;
                    UpdateQuantityDisplay(product, currentQty);
                }
                else
                {
                    MessageBox.Show($"На складе только {product.Stock} шт.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.Tag as FlowerProduct;
            if (product != null)
            {
                int currentQty = _selectedQuantities.ContainsKey(product.Id) ? _selectedQuantities[product.Id] : 1;
                if (currentQty > 1)
                {
                    currentQty--;
                    _selectedQuantities[product.Id] = currentQty;
                    UpdateQuantityDisplay(product, currentQty);
                }
            }
        }

        private void UpdateQuantityDisplay(FlowerProduct product, int quantity)
        {
            var container = ProductsListView.ItemContainerGenerator.ContainerFromItem(product) as FrameworkElement;
            if (container != null)
            {
                var quantityText = FindVisualChild<TextBlock>(container, "QuantityText");
                if (quantityText != null)
                {
                    quantityText.Text = quantity.ToString();
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t && t.Name == name)
                    return t;

                var result = FindVisualChild<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void AddToCartWithQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.Tag as FlowerProduct;
            if (product != null)
            {
                int quantity = _selectedQuantities.ContainsKey(product.Id) ? _selectedQuantities[product.Id] : 1;

                if (quantity > product.Stock)
                {
                    MessageBox.Show($"Недостаточно товара на складе! Доступно: {product.Stock} шт.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var existingItem = _cart.FirstOrDefault(c => c.Product.Id == product.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    _cart.Add(new CartItem { Product = product, Quantity = quantity });
                }

                product.Stock -= quantity;

                try
                {
                    _dbHelper.UpdateProductStock(product.Id, product.Stock);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления склада: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                _selectedQuantities[product.Id] = 1;
                UpdateQuantityDisplay(product, 1);
                LoadProducts();

                MessageBox.Show($"{product.Name} ({quantity} шт.) добавлен в корзину!",
                    "Корзина", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateCartDisplay();
            }
        }

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var cartItem = button?.Tag as CartItem;
            if (cartItem != null)
            {
                cartItem.Product.Stock += cartItem.Quantity;

                try
                {
                    _dbHelper.UpdateProductStock(cartItem.Product.Id, cartItem.Product.Stock);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления склада: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                _cart.Remove(cartItem);
                UpdateCartDisplay();
                LoadProducts();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddEditProduct(_shop));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListView.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар для редактирования", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedProduct = ProductsListView.SelectedItem as FlowerProduct;
            NavigationService?.Navigate(new AddEditProduct(_shop, selectedProduct));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListView.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар для удаления", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedProduct = ProductsListView.SelectedItem as FlowerProduct;

            var result = MessageBox.Show($"Удалить товар \"{selectedProduct.Name}\"?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _shop.DeleteFlower(selectedProduct.Id);
                LoadDataFromDatabase();
                LoadProducts();
                MessageBox.Show("Товар успешно удален!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private string CreateQRCodeImage(string text)
        {
            string tempFilePath = Path.GetTempFileName() + ".png";

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                    {
                        qrBitmap.Save(tempFilePath, ImageFormat.Png);
                    }
                }
            }

            return tempFilePath;
        }

        private void ShowPdfReceipt(OrderInfo order)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (PdfDocument document = new PdfDocument())
                    {
                        document.Info.Title = $"Чек заказа №{order.OrderNumber}";
                        document.Info.Author = "FLOWWOW";

                        PdfPage page = document.AddPage();
                        page.Width = 450;
                        page.Height = 750;

                        using (XGraphics gfx = XGraphics.FromPdfPage(page))
                        {
                            XFont titleFont = new XFont("Arial", 24, XFontStyle.Bold);
                            XFont headerFont = new XFont("Arial", 16, XFontStyle.Bold);
                            XFont normalFont = new XFont("Arial", 11, XFontStyle.Regular);
                            XFont boldFont = new XFont("Arial", 11, XFontStyle.Bold);
                            XFont smallFont = new XFont("Arial", 9, XFontStyle.Regular);
                            XFont priceFont = new XFont("Arial", 14, XFontStyle.Bold);
                            XFont orderNumFont = new XFont("Arial", 12, XFontStyle.Bold);

                            XColor pastelGreen = XColor.FromArgb(156, 158, 74);
                            XColor softPink = XColor.FromArgb(248, 150, 163);
                            XColor mainBlack = XColor.FromArgb(36, 32, 33);
                            XColor lightBrown = XColor.FromArgb(204, 190, 131);

                            int yPos = 30;
                            int leftMargin = 30;
                            int rightMargin = 420;

                            gfx.DrawString("🌸 FLOWWOW", titleFont, new XSolidBrush(pastelGreen),
                                new XRect(0, yPos, page.Width, 30), XStringFormats.TopCenter);
                            yPos += 35;
                            gfx.DrawString("Цветочный магазин", smallFont, new XSolidBrush(lightBrown),
                                new XRect(0, yPos, page.Width, 20), XStringFormats.TopCenter);
                            yPos += 25;
                            gfx.DrawLine(new XPen(lightBrown, 1), leftMargin, yPos, rightMargin, yPos);
                            yPos += 15;
                            gfx.DrawString("ЧЕК ЗАКАЗА", headerFont, new XSolidBrush(mainBlack),
                                new XRect(0, yPos, page.Width, 25), XStringFormats.TopCenter);
                            yPos += 30;

                            gfx.DrawString("Номер заказа:", boldFont, XBrushes.Black, leftMargin, yPos);
                            gfx.DrawString($"#{order.OrderNumber}", orderNumFont, new XSolidBrush(softPink), 180, yPos);
                            yPos += 20;
                            gfx.DrawString("Дата:", boldFont, XBrushes.Black, leftMargin, yPos);
                            gfx.DrawString(order.OrderDate, normalFont, XBrushes.Black, 180, yPos);
                            yPos += 20;
                            gfx.DrawString("Способ доставки:", boldFont, XBrushes.Black, leftMargin, yPos);
                            gfx.DrawString(order.DeliveryMethod, normalFont, new XSolidBrush(pastelGreen), 180, yPos);
                            yPos += 20;

                            if (order.DeliveryMethod == "Доставка")
                            {
                                gfx.DrawString("Адрес доставки:", boldFont, XBrushes.Black, leftMargin, yPos);
                                string address = order.DeliveryAddress.Length > 30 ? order.DeliveryAddress.Substring(0, 27) + "..." : order.DeliveryAddress;
                                gfx.DrawString(address, normalFont, XBrushes.Black, 180, yPos);
                                yPos += 20;
                            }

                            gfx.DrawString("Статус:", boldFont, XBrushes.Black, leftMargin, yPos);
                            gfx.DrawString("ОПЛАЧЕН", normalFont, new XSolidBrush(pastelGreen), 180, yPos);
                            yPos += 25;
                            gfx.DrawLine(new XPen(lightBrown, 1), leftMargin, yPos, rightMargin, yPos);
                            yPos += 15;
                            gfx.DrawString("Товары в заказе:", boldFont, XBrushes.Black, leftMargin, yPos);
                            yPos += 20;

                            gfx.DrawRectangle(new XSolidBrush(pastelGreen), leftMargin, yPos, 200, 18);
                            gfx.DrawRectangle(new XSolidBrush(pastelGreen), leftMargin + 205, yPos, 70, 18);
                            gfx.DrawRectangle(new XSolidBrush(pastelGreen), leftMargin + 280, yPos, 110, 18);
                            gfx.DrawString("Наименование", boldFont, XBrushes.White, leftMargin + 5, yPos + 13);
                            gfx.DrawString("Кол-во", boldFont, XBrushes.White, leftMargin + 235, yPos + 13);
                            gfx.DrawString("Сумма", boldFont, XBrushes.White, leftMargin + 325, yPos + 13);
                            yPos += 25;

                            foreach (var item in order.Items)
                            {
                                decimal total = item.Product.Price * item.Quantity;
                                string name = item.Product.Name.Length > 28 ? item.Product.Name.Substring(0, 25) + "..." : item.Product.Name;
                                gfx.DrawString(name, normalFont, XBrushes.Black, leftMargin, yPos);
                                gfx.DrawString($"{item.Quantity} шт.", normalFont, XBrushes.Black, leftMargin + 230, yPos);
                                gfx.DrawString($"{total:N0} ₽", normalFont, XBrushes.Black, leftMargin + 330, yPos);
                                yPos += 20;
                            }

                            yPos += 10;
                            gfx.DrawLine(new XPen(lightBrown, 1), leftMargin, yPos, rightMargin, yPos);
                            yPos += 15;
                            gfx.DrawString("Сумма товаров:", normalFont, XBrushes.Black, leftMargin + 180, yPos);
                            gfx.DrawString($"{order.TotalAmount:N0} ₽", normalFont, XBrushes.Black, leftMargin + 340, yPos);
                            yPos += 20;

                            if (order.DeliveryPrice > 0)
                            {
                                gfx.DrawString("Доставка:", normalFont, XBrushes.Black, leftMargin + 180, yPos);
                                gfx.DrawString($"{order.DeliveryPrice:N0} ₽", normalFont, XBrushes.Black, leftMargin + 340, yPos);
                                yPos += 20;
                            }

                            gfx.DrawLine(new XPen(lightBrown, 1), leftMargin, yPos, rightMargin, yPos);
                            yPos += 15;
                            decimal finalTotal = order.FinalTotal;
                            gfx.DrawString("ИТОГО К ОПЛАТЕ:", boldFont, XBrushes.Black, leftMargin + 180, yPos);
                            gfx.DrawString($"{finalTotal:N0} ₽", priceFont, new XSolidBrush(pastelGreen), leftMargin + 340, yPos);
                            yPos += 40;
                            gfx.DrawLine(new XPen(lightBrown, 1), leftMargin, yPos, rightMargin, yPos);
                            yPos += 20;

                            gfx.DrawString("📱 Отсканируйте QR-код", boldFont, XBrushes.Black, new XRect(0, yPos, page.Width, 20), XStringFormats.TopCenter);
                            yPos += 15;
                            gfx.DrawString("для получения информации о заказе", smallFont, new XSolidBrush(lightBrown), new XRect(0, yPos, page.Width, 20), XStringFormats.TopCenter);
                            yPos += 15;

                            string qrText = $"Заказ №{order.OrderNumber}\nДата: {order.OrderDate}\nСумма товаров: {order.TotalAmount:N0} ₽\nДоставка: {order.DeliveryMethod}\nИтого: {finalTotal:N0} ₽\nСпасибо за покупку в FLOWWOW!";
                            string tempQrPath = CreateQRCodeImage(qrText);
                            XImage qrImage = XImage.FromFile(tempQrPath);
                            int qrSize = 120;
                            int qrX = (int)((page.Width - qrSize) / 2);
                            gfx.DrawImage(qrImage, qrX, yPos, qrSize, qrSize);
                            qrImage.Dispose();
                            File.Delete(tempQrPath);
                            yPos += 130;

                            gfx.DrawString("🌸 Спасибо за покупку!", boldFont, new XSolidBrush(pastelGreen), new XRect(0, yPos, page.Width, 20), XStringFormats.TopCenter);
                            yPos += 20;
                            gfx.DrawString("Ждем вас снова в FLOWWOW", smallFont, new XSolidBrush(lightBrown), new XRect(0, yPos, page.Width, 20), XStringFormats.TopCenter);
                            yPos += 20;
                            gfx.DrawLine(new XPen(lightBrown, 1), leftMargin, yPos, rightMargin, yPos);
                        }

                        document.Save(ms, false);
                    }

                    string tempPdfPath = Path.GetTempFileName() + ".pdf";
                    File.WriteAllBytes(tempPdfPath, ms.ToArray());
                    System.Diagnostics.Process.Start(tempPdfPath);

                    MessageBox.Show($"Чек заказа №{order.OrderNumber} открыт для просмотра!",
                        "PDF Чек", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании PDF: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ShowDeliveryDialog(out string deliveryMethod, out string deliveryAddress)
        {
            string tempDeliveryMethod = "Самовывоз";
            string tempDeliveryAddress = "";

            Window dialog = new Window
            {
                Title = "Выберите способ получения",
                Width = 400,
                Height = 350,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Background = System.Windows.Media.Brushes.White
            };

            StackPanel panel = new StackPanel { Margin = new Thickness(20) };

            panel.Children.Add(new TextBlock
            {
                Text = "Способ получения заказа",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(MediaColor.FromRgb(156, 158, 74)),
                Margin = new Thickness(0, 0, 0, 15)
            });

            RadioButton pickupRadio = new RadioButton { Content = "🏪 Самовывоз (бесплатно)", FontSize = 14, Margin = new Thickness(0, 5, 0, 5), IsChecked = true };
            RadioButton deliveryRadio = new RadioButton { Content = "🚚 Доставка (+500 ₽)", FontSize = 14, Margin = new Thickness(0, 5, 0, 5) };

            panel.Children.Add(pickupRadio);
            panel.Children.Add(deliveryRadio);

            TextBlock addressLabel = new TextBlock { Text = "Адрес доставки:", FontSize = 12, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 15, 0, 5), Visibility = Visibility.Collapsed };
            TextBox addressBox = new TextBox { Margin = new Thickness(0, 0, 0, 10), Padding = new Thickness(8), FontSize = 12, Visibility = Visibility.Collapsed };

            panel.Children.Add(addressLabel);
            panel.Children.Add(addressBox);

            deliveryRadio.Checked += (s, e) => { addressLabel.Visibility = Visibility.Visible; addressBox.Visibility = Visibility.Visible; };
            pickupRadio.Checked += (s, e) => { addressLabel.Visibility = Visibility.Collapsed; addressBox.Visibility = Visibility.Collapsed; };

            StackPanel buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) };

            Button okBtn = new Button
            {
                Content = "Оформить",
                Width = 100,
                Height = 35,
                Margin = new Thickness(5),
                Background = new SolidColorBrush(MediaColor.FromRgb(156, 158, 74)),
                Foreground = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            Button cancelBtn = new Button
            {
                Content = "Отмена",
                Width = 100,
                Height = 35,
                Margin = new Thickness(5),
                Background = System.Windows.Media.Brushes.LightGray,
                Foreground = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            buttonPanel.Children.Add(okBtn);
            buttonPanel.Children.Add(cancelBtn);
            panel.Children.Add(buttonPanel);

            dialog.Content = panel;

            bool result = false;
            okBtn.Click += (s, e) =>
            {
                if (deliveryRadio.IsChecked == true && string.IsNullOrWhiteSpace(addressBox.Text))
                {
                    MessageBox.Show("Пожалуйста, укажите адрес доставки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                tempDeliveryMethod = pickupRadio.IsChecked == true ? "Самовывоз" : "Доставка";
                tempDeliveryAddress = addressBox.Text;
                result = true;
                dialog.Close();
            };

            cancelBtn.Click += (s, e) => dialog.Close();

            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();

            deliveryMethod = tempDeliveryMethod;
            deliveryAddress = tempDeliveryAddress;
            return result;
        }

        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentUser == null)
            {
                MessageBox.Show("Пожалуйста, войдите в систему!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ShowDeliveryDialog(out string deliveryMethod, out string deliveryAddress))
                return;

            try
            {
                int orderNumber = _orders.Count + 1;
                decimal totalAmount = _cart.Sum(c => c.Product.Price * c.Quantity);
                decimal deliveryPrice = deliveryMethod == "Доставка" ? 500 : 0;

                var cartItemsForDb = _cart.Select(c => new CartItemInfo
                {
                    FlowerId = c.Product.Id,
                    Quantity = c.Quantity,
                    Price = c.Product.Price,
                    DiscountPercent = c.Product.DiscountPercent,
                    Name = c.Product.Name,
                    Stock = c.Product.Stock
                }).ToList();

                int orderId = _dbHelper.CreateOrder(_currentUser.Id, deliveryMethod, deliveryAddress, cartItemsForDb, totalAmount);
                _dbHelper.AddPayment(orderId, totalAmount + deliveryPrice, "Банковская карта");

                var order = new OrderInfo
                {
                    OrderNumber = orderId,
                    OrderDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                    ItemsCount = $"{_cart.Sum(c => c.Quantity)} товара(ов)",
                    TotalAmount = totalAmount,
                    Items = new List<CartItem>(_cart),
                    DeliveryMethod = deliveryMethod,
                    DeliveryPrice = deliveryPrice,
                    DeliveryAddress = deliveryAddress
                };

                _orders.Add(order);
                _cart.Clear();
                UpdateCartDisplay();
                LoadDataFromDatabase();
                LoadProducts();
                ShowPdfReceipt(order);

                MessageBox.Show($"Заказ №{orderId} оформлен!\nСумма к оплате: {(totalAmount + deliveryPrice):N0} ₽",
                    "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                BtnBackToCatalog_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBackToCatalog_Click(object sender, RoutedEventArgs e)
        {
            CatalogView.Visibility = Visibility.Visible;
            CartView.Visibility = Visibility.Collapsed;
            HistoryView.Visibility = Visibility.Collapsed;
        }

        private void BtnBackFromHistory_Click(object sender, RoutedEventArgs e)
        {
            CatalogView.Visibility = Visibility.Visible;
            CartView.Visibility = Visibility.Collapsed;
            HistoryView.Visibility = Visibility.Collapsed;
        }

        private void UpdateCartDisplay()
        {
            CartListBox.ItemsSource = null;
            CartListBox.ItemsSource = _cart;
            decimal total = _cart.Sum(c => c.Product.Price * c.Quantity);
            CartTotalText.Text = $"{total:N0} ₽";
        }

        private void UpdateHistoryDisplay()
        {
            HistoryListBox.ItemsSource = null;
            HistoryListBox.ItemsSource = _orders;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
            {
                var window = Window.GetWindow(this);
                if (window != null)
                    window.Close();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataFromDatabase();
            LoadProducts();
        }
    }
}