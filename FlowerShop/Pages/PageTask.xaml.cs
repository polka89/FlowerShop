using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.IO;
using FlowerShop.ApplicationData;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
                UpdateCartDisplay();
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

        // ==================== ГЕНЕРАЦИЯ QR-КОДА ====================
        private System.Windows.Media.Imaging.BitmapImage GenerateQRCode(string text)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                    {
                        using (MemoryStream memory = new MemoryStream())
                        {
                            qrBitmap.Save(memory, ImageFormat.Png);
                            memory.Position = 0;

                            System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = memory;
                            bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                            bitmapImage.EndInit();

                            return bitmapImage;
                        }
                    }
                }
            }
        }

        // ==================== СОЗДАНИЕ PDF ЧЕКА С КРАСИВЫМ ОФОРМЛЕНИЕМ ====================
        private void CreatePdfReceipt(OrderInfo order)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = $"Чек_заказ_{order.OrderNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(desktopPath, fileName);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    iTextSharp.text.Rectangle pageSize = new iTextSharp.text.Rectangle(450, 700);
                    Document document = new Document(pageSize);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    document.Open();

                    // Цвета
                    BaseColor pastelGreen = new BaseColor(156, 158, 74);
                    BaseColor softPink = new BaseColor(248, 150, 163);
                    BaseColor mainBlack = new BaseColor(36, 32, 33);
                    BaseColor lightBrown = new BaseColor(204, 190, 131);

                    // Шрифты
                    iTextSharp.text.Font titleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 24, iTextSharp.text.Font.BOLD, pastelGreen);
                    iTextSharp.text.Font headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD, mainBlack);
                    iTextSharp.text.Font normalFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.NORMAL, mainBlack);
                    iTextSharp.text.Font boldFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, mainBlack);
                    iTextSharp.text.Font smallFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.NORMAL, lightBrown);
                    iTextSharp.text.Font priceFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD, pastelGreen);
                    iTextSharp.text.Font orderNumFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, softPink);

                    // ========== ВЕРХНЯЯ ЧАСТЬ С ЛОГОТИПОМ ==========
                    Paragraph logo = new Paragraph("🌸 FLOWWOW", titleFont);
                    logo.Alignment = Element.ALIGN_CENTER;
                    document.Add(logo);

                    Paragraph subtitle = new Paragraph("Цветочный магазин", smallFont);
                    subtitle.Alignment = Element.ALIGN_CENTER;
                    document.Add(subtitle);
                    document.Add(new Paragraph(" "));

                    // Декоративная линия
                    PdfPTable lineTable = new PdfPTable(1);
                    lineTable.WidthPercentage = 100;
                    PdfPCell lineCell = new PdfPCell();
                    lineCell.Border = PdfPCell.BOTTOM_BORDER;
                    lineCell.BorderColor = lightBrown;
                    lineCell.BorderWidth = 1f;
                    lineCell.Padding = 5;
                    lineTable.AddCell(lineCell);
                    document.Add(lineTable);
                    document.Add(new Paragraph(" "));

                    // ========== ЗАГОЛОВОК ЧЕКА ==========
                    Paragraph receiptTitle = new Paragraph("ЧЕК ЗАКАЗА", headerFont);
                    receiptTitle.Alignment = Element.ALIGN_CENTER;
                    document.Add(receiptTitle);
                    document.Add(new Paragraph(" "));

                    // ========== ИНФОРМАЦИЯ О ЗАКАЗЕ В ТАБЛИЦЕ ==========
                    PdfPTable infoTable = new PdfPTable(2);
                    infoTable.WidthPercentage = 100;
                    infoTable.SetWidths(new float[] { 1f, 2f });

                    PdfPCell orderNumLabel = new PdfPCell(new Phrase("Номер заказа:", boldFont));
                    orderNumLabel.Border = PdfPCell.NO_BORDER;
                    orderNumLabel.Padding = 5;
                    infoTable.AddCell(orderNumLabel);

                    PdfPCell orderNumValue = new PdfPCell(new Phrase($"#{order.OrderNumber}", orderNumFont));
                    orderNumValue.Border = PdfPCell.NO_BORDER;
                    orderNumValue.Padding = 5;
                    infoTable.AddCell(orderNumValue);

                    PdfPCell dateLabel = new PdfPCell(new Phrase("Дата:", boldFont));
                    dateLabel.Border = PdfPCell.NO_BORDER;
                    dateLabel.Padding = 5;
                    infoTable.AddCell(dateLabel);

                    PdfPCell dateValue = new PdfPCell(new Phrase(order.OrderDate, normalFont));
                    dateValue.Border = PdfPCell.NO_BORDER;
                    dateValue.Padding = 5;
                    infoTable.AddCell(dateValue);

                    PdfPCell statusLabel = new PdfPCell(new Phrase("Статус:", boldFont));
                    statusLabel.Border = PdfPCell.NO_BORDER;
                    statusLabel.Padding = 5;
                    infoTable.AddCell(statusLabel);

                    PdfPCell statusValue = new PdfPCell(new Phrase("✅ ОПЛАЧЕН", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11, iTextSharp.text.Font.BOLD, pastelGreen)));
                    statusValue.Border = PdfPCell.NO_BORDER;
                    statusValue.Padding = 5;
                    infoTable.AddCell(statusValue);

                    document.Add(infoTable);
                    document.Add(new Paragraph(" "));

                    // Декоративная линия
                    document.Add(lineTable);
                    document.Add(new Paragraph(" "));

                    // ========== ТАБЛИЦА ТОВАРОВ ==========
                    Paragraph itemsHeader = new Paragraph("Товары в заказе", boldFont);
                    itemsHeader.Alignment = Element.ALIGN_LEFT;
                    document.Add(itemsHeader);
                    document.Add(new Paragraph(" "));

                    PdfPTable itemsTable = new PdfPTable(3);
                    itemsTable.WidthPercentage = 100;
                    itemsTable.SetWidths(new float[] { 2.5f, 1f, 1.5f });
                    itemsTable.SpacingBefore = 5f;
                    itemsTable.SpacingAfter = 5f;

                    iTextSharp.text.Font tableHeaderFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                    PdfPCell headerCell1 = new PdfPCell(new Phrase("Наименование", tableHeaderFont));
                    headerCell1.BackgroundColor = pastelGreen;
                    headerCell1.Padding = 8;
                    headerCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    itemsTable.AddCell(headerCell1);

                    PdfPCell headerCell2 = new PdfPCell(new Phrase("Кол-во", tableHeaderFont));
                    headerCell2.BackgroundColor = pastelGreen;
                    headerCell2.Padding = 8;
                    headerCell2.HorizontalAlignment = Element.ALIGN_CENTER;
                    itemsTable.AddCell(headerCell2);

                    PdfPCell headerCell3 = new PdfPCell(new Phrase("Сумма", tableHeaderFont));
                    headerCell3.BackgroundColor = pastelGreen;
                    headerCell3.Padding = 8;
                    headerCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                    itemsTable.AddCell(headerCell3);

                    // Группировка товаров
                    var grouped = order.Items
                        .GroupBy(x => x.Id)
                        .Select(g => new { Product = g.First(), Quantity = g.Count() })
                        .ToList();

                    foreach (var item in grouped)
                    {
                        decimal total = item.Product.Price * item.Quantity;
                        string name = item.Product.Name.Length > 30 ? item.Product.Name.Substring(0, 27) + "..." : item.Product.Name;

                        PdfPCell nameCell = new PdfPCell(new Phrase(name, normalFont));
                        nameCell.Padding = 6;
                        nameCell.Border = PdfPCell.BOTTOM_BORDER;
                        nameCell.BorderColor = lightBrown;
                        nameCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        itemsTable.AddCell(nameCell);

                        PdfPCell qtyCell = new PdfPCell(new Phrase($"{item.Quantity} шт.", normalFont));
                        qtyCell.Padding = 6;
                        qtyCell.Border = PdfPCell.BOTTOM_BORDER;
                        qtyCell.BorderColor = lightBrown;
                        qtyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        itemsTable.AddCell(qtyCell);

                        PdfPCell priceCell = new PdfPCell(new Phrase($"{total:N0} ₽", normalFont));
                        priceCell.Padding = 6;
                        priceCell.Border = PdfPCell.BOTTOM_BORDER;
                        priceCell.BorderColor = lightBrown;
                        priceCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        itemsTable.AddCell(priceCell);
                    }

                    document.Add(itemsTable);
                    document.Add(new Paragraph(" "));

                    // ========== ИТОГОВАЯ СУММА ==========
                    PdfPTable totalTable = new PdfPTable(2);
                    totalTable.WidthPercentage = 100;
                    totalTable.SetWidths(new float[] { 2f, 1f });

                    PdfPCell totalLabelCell = new PdfPCell(new Phrase("ИТОГО К ОПЛАТЕ:", boldFont));
                    totalLabelCell.Border = PdfPCell.NO_BORDER;
                    totalLabelCell.Padding = 8;
                    totalLabelCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    totalTable.AddCell(totalLabelCell);

                    PdfPCell totalValueCell = new PdfPCell(new Phrase($"{order.TotalAmount:N0} ₽", priceFont));
                    totalValueCell.Border = PdfPCell.NO_BORDER;
                    totalValueCell.Padding = 8;
                    totalValueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    totalTable.AddCell(totalValueCell);

                    document.Add(totalTable);
                    document.Add(new Paragraph(" "));

                    // Декоративная линия
                    document.Add(lineTable);
                    document.Add(new Paragraph(" "));

                    // ========== QR-КОД ==========
                    Paragraph qrTitle = new Paragraph("📱 Отсканируйте QR-код", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, mainBlack));
                    qrTitle.Alignment = Element.ALIGN_CENTER;
                    document.Add(qrTitle);

                    Paragraph qrSubtitle = new Paragraph("для получения информации о заказе", smallFont);
                    qrSubtitle.Alignment = Element.ALIGN_CENTER;
                    document.Add(qrSubtitle);
                    document.Add(new Paragraph(" "));

                    string qrText = $"Заказ №{order.OrderNumber}\nДата: {order.OrderDate}\nСумма: {order.TotalAmount:N0} ₽\nСпасибо за покупку в FLOWWOW!";

                    using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                    {
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
                        using (QRCode qrCode = new QRCode(qrCodeData))
                        {
                            using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                            {
                                string tempQrPath = Path.GetTempFileName() + ".png";
                                qrBitmap.Save(tempQrPath, ImageFormat.Png);

                                iTextSharp.text.Image qrImage = iTextSharp.text.Image.GetInstance(tempQrPath);
                                qrImage.ScaleToFit(120, 120);
                                qrImage.Alignment = Element.ALIGN_CENTER;
                                document.Add(qrImage);

                                File.Delete(tempQrPath);
                            }
                        }
                    }

                    document.Add(new Paragraph(" "));

                    // ========== НИЖНЯЯ ЧАСТЬ ==========
                    Paragraph thanks = new Paragraph("🌸 Спасибо за покупку!", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, pastelGreen));
                    thanks.Alignment = Element.ALIGN_CENTER;
                    document.Add(thanks);

                    Paragraph welcome = new Paragraph("Ждем вас снова в FLOWWOW", smallFont);
                    welcome.Alignment = Element.ALIGN_CENTER;
                    document.Add(welcome);
                    document.Add(new Paragraph(" "));

                    // Декоративная линия внизу
                    document.Add(lineTable);

                    document.Close();
                }

                System.Diagnostics.Process.Start("explorer.exe", "/select," + filePath);
                MessageBox.Show($"PDF чек сохранен на рабочий стол!\n{fileName}", "PDF Чек", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании PDF: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ==================== ОФОРМЛЕНИЕ ЗАКАЗА ====================
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
            _cart.Clear();
            UpdateCartDisplay();

            // СОЗДАЁМ PDF ЧЕК
            CreatePdfReceipt(order);

            MessageBox.Show($"Заказ №{orderNumber} оформлен!\nPDF чек сохранен на рабочий стол.",
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

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                NavigationService?.Navigate(new Auth());
            }
        }

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