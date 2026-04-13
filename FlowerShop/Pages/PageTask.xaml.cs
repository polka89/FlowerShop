using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.IO;
using FlowerShop.ApplicationData;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using QRCoder;

namespace FlowerShop.Pages
{
    // Конвертер для изображений
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

    public partial class PageTask : Page
    {
        private ShopModel _shop;
        private int _currentPage = 1;
        private int _itemsPerPage = 6;
        private List<FlowerProduct> _allProducts;
        private int? _selectedCategoryId = null;

        private List<FlowerProduct> _cart = new List<FlowerProduct>();
        private List<OrderInfo> _orders = new List<OrderInfo>();

        public PageTask()
        {
            InitializeComponent();
            _shop = new ShopModel();
            _allProducts = _shop.Flowers.Where(f => f.IsAvailable).ToList();
            LoadCategories();
            LoadProducts();
        }

        public class OrderInfo
        {
            public string OrderDate { get; set; }
            public string ItemsCount { get; set; }
            public decimal TotalAmount { get; set; }
            public List<FlowerProduct> Items { get; set; }
            public int OrderNumber { get; set; }
        }

        private void LoadCategories()
        {
            CategoriesPanel.ItemsSource = _shop.Categories;
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

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.Tag as FlowerProduct;
            if (product != null)
            {
                _cart.Add(product);
                MessageBox.Show($"{product.Name} добавлен в корзину!", "Корзина", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var product = button?.Tag as FlowerProduct;
            if (product != null)
            {
                _cart.Remove(product);
                UpdateCartDisplay();
            }
        }

        // --------------------------------------------------------------
        // Генерация PDF чека
        // --------------------------------------------------------------
        private void CreatePdfReceipt(OrderInfo order)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = $"Чек_заказ_{order.OrderNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(desktopPath, fileName);

                using (PdfDocument document = new PdfDocument())
                {
                    document.Info.Title = $"Чек заказа №{order.OrderNumber}";
                    document.Info.Author = "FLOWWOW";

                    PdfPage page = document.AddPage();
                    page.Width = 400;
                    page.Height = 600;

                    using (XGraphics gfx = XGraphics.FromPdfPage(page))
                    {
                        XFont titleFont = new XFont("Arial", 18, XFontStyle.Bold);
                        XFont headerFont = new XFont("Arial", 14, XFontStyle.Bold);
                        XFont normalFont = new XFont("Arial", 10, XFontStyle.Regular);
                        XFont boldFont = new XFont("Arial", 10, XFontStyle.Bold);

                        int yPos = 30;
                        int leftMargin = 20;

                        gfx.DrawString("🌸 FLOWWOW", titleFont, XBrushes.Black,
                            new XRect(0, yPos, page.Width, 30), XStringFormats.TopCenter);
                        yPos += 35;

                        gfx.DrawString("ЧЕК ЗАКАЗА", headerFont, XBrushes.Black,
                            new XRect(0, yPos, page.Width, 25), XStringFormats.TopCenter);
                        yPos += 30;

                        gfx.DrawLine(XPens.Black, leftMargin, yPos, page.Width - leftMargin, yPos);
                        yPos += 15;

                        gfx.DrawString($"Номер заказа: #{order.OrderNumber}", normalFont, XBrushes.Black, leftMargin, yPos);
                        yPos += 20;
                        gfx.DrawString($"Дата: {order.OrderDate}", normalFont, XBrushes.Black, leftMargin, yPos);
                        yPos += 20;

                        gfx.DrawLine(XPens.Black, leftMargin, yPos, page.Width - leftMargin, yPos);
                        yPos += 15;

                        gfx.DrawString("Товар", boldFont, XBrushes.Black, leftMargin, yPos);
                        gfx.DrawString("Кол-во", boldFont, XBrushes.Black, 220, yPos);
                        gfx.DrawString("Цена", boldFont, XBrushes.Black, 290, yPos);
                        yPos += 20;

                        var grouped = order.Items
                            .GroupBy(x => x.Id)
                            .Select(g => new { Product = g.First(), Quantity = g.Count() })
                            .ToList();

                        foreach (var item in grouped)
                        {
                            decimal total = item.Product.Price * item.Quantity;
                            string name = item.Product.Name.Length > 25 ? item.Product.Name.Substring(0, 22) + "..." : item.Product.Name;
                            gfx.DrawString(name, normalFont, XBrushes.Black, leftMargin, yPos);
                            gfx.DrawString($"{item.Quantity} шт.", normalFont, XBrushes.Black, 220, yPos);
                            gfx.DrawString($"{total:N0} ₽", normalFont, XBrushes.Black, 290, yPos);
                            yPos += 18;
                        }

                        yPos += 10;
                        gfx.DrawLine(XPens.Black, leftMargin, yPos, page.Width - leftMargin, yPos);
                        yPos += 15;

                        gfx.DrawString("ИТОГО:", boldFont, XBrushes.Black, 200, yPos);
                        gfx.DrawString($"{order.TotalAmount:N0} ₽", boldFont, XBrushes.Purple, 290, yPos);
                        yPos += 30;

                        gfx.DrawString("Спасибо за покупку!", normalFont, XBrushes.Black,
                            new XRect(0, yPos, page.Width, 20), XStringFormats.TopCenter);
                        yPos += 20;
                        gfx.DrawString("Ждем вас снова в FLOWWOW", normalFont, XBrushes.Gray,
                            new XRect(0, yPos, page.Width, 20), XStringFormats.TopCenter);
                    }

                    document.Save(filePath);
                }

                System.Diagnostics.Process.Start("explorer.exe", "/select," + filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании PDF: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --------------------------------------------------------------
        // Показать окно "Спасибо" с QR-кодом
        // --------------------------------------------------------------
        private void ShowThankYouWindow(int orderNumber, decimal totalAmount)
        {
            // Ссылка на страницу отзыва (замените на реальную)
            string reviewLink = "https://forms.gle/ваша_ссылка_на_отзыв";

            Window thankYouWindow = new Window
            {
                Title = "Спасибо за заказ!",
                Width = 400,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Background = System.Windows.Media.Brushes.White
            };

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(reviewLink, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    System.Drawing.Bitmap qrBitmap = qrCode.GetGraphic(20);
                    using (MemoryStream memory = new MemoryStream())
                    {
                        qrBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                        memory.Position = 0;
                        System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();

                        StackPanel panel = new StackPanel { Margin = new Thickness(20) };
                        panel.Children.Add(new TextBlock
                        {
                            Text = $"✅ Заказ №{orderNumber} оформлен!",
                            FontSize = 18,
                            FontWeight = FontWeights.Bold,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Margin = new Thickness(0, 0, 0, 10)
                        });
                        panel.Children.Add(new TextBlock
                        {
                            Text = $"Сумма: {totalAmount:N0} ₽",
                            FontSize = 14,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Margin = new Thickness(0, 0, 0, 20)
                        });

                        Image qrImage = new Image
                        {
                            Source = bitmapImage,
                            Width = 200,
                            Height = 200,
                            Margin = new Thickness(0, 0, 0, 20),
                            HorizontalAlignment = HorizontalAlignment.Center
                        };
                        panel.Children.Add(qrImage);

                        panel.Children.Add(new TextBlock
                        {
                            Text = "Отсканируйте QR-код, чтобы оставить отзыв",
                            FontSize = 12,
                            Foreground = System.Windows.Media.Brushes.Gray,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Margin = new Thickness(0, 0, 0, 15)
                        });

                        Button closeBtn = new Button
                        {
                            Content = "Закрыть",
                            Width = 100,
                            Height = 35,
                            Margin = new Thickness(0, 10, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Background = (System.Windows.Media.Brush)this.Resources["PastelGreen"],
                            Foreground = System.Windows.Media.Brushes.White,
                            BorderThickness = new Thickness(0),
                            Cursor = System.Windows.Input.Cursors.Hand
                        };
                        closeBtn.Click += (s, e) => thankYouWindow.Close();
                        panel.Children.Add(closeBtn);

                        thankYouWindow.Content = panel;
                    }
                }
            }

            thankYouWindow.Owner = Window.GetWindow(this);
            thankYouWindow.ShowDialog();
        }

        // --------------------------------------------------------------
        // Оформление заказа
        // --------------------------------------------------------------
        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int orderNumber = _orders.Count + 1;
            decimal totalAmount = _cart.Sum(p => p.Price);

            var order = new OrderInfo
            {
                OrderNumber = orderNumber,
                OrderDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                ItemsCount = $"{_cart.Count} товара(ов)",
                TotalAmount = totalAmount,
                Items = new List<FlowerProduct>(_cart)
            };

            _orders.Add(order);
            CreatePdfReceipt(order);               // PDF чек
            _cart.Clear();
            UpdateCartDisplay();

            ShowThankYouWindow(orderNumber, totalAmount);   // QR-окно

            MessageBox.Show($"Заказ №{orderNumber} оформлен!\nЧек сохранен на рабочий стол.",
                "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

            BtnBackToCatalog_Click(sender, e);
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
            CartTotalText.Text = $"{_cart.Sum(p => p.Price):N0} ₽";
        }

        private void UpdateHistoryDisplay()
        {
            HistoryListBox.ItemsSource = null;
            HistoryListBox.ItemsSource = _orders;
        }

        // --------------------------------------------------------------
        // Кнопки управления (добавить, редактировать, удалить, назад)
        // --------------------------------------------------------------
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Добавление товара", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListView.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар для редактирования", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBox.Show("Редактирование товара", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListView.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар для удаления", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Удалить выбранный товар?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Товар удален", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
    }
}