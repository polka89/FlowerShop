using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using FlowerShop.ApplicationData;

namespace FlowerShop.Pages
{
    public partial class AddRecip : Page
    {
        private readonly int _productId;
        private FlowerProduct _currentProduct;
        private List<string> _images = new List<string>();
        private int _currentIndex;

        public AddRecip(int id)
        {
            InitializeComponent();
            _productId = id;
            InitializeData();
        }

        private void InitializeData()
        {
            CbCategory.ItemsSource = AppConnect.model01.Categories.OrderBy(c => c.Name).ToList();

            if (_productId == 0)
            {
                _currentProduct = new FlowerProduct
                {
                    IsAvailable = true
                };
            }
            else
            {
                _currentProduct = AppConnect.model01.Flowers.FirstOrDefault(f => f.Id == _productId);
                if (_currentProduct == null)
                {
                    MessageBox.Show("Товар не найден.");
                    AppFrame.frmMain.Navigate(new PageTask());
                    return;
                }
            }

            DataContext = _currentProduct;
            CbCategory.SelectedValue = _currentProduct.CategoryId;
            RefreshImages();
        }

        private void CbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_currentProduct != null && CbCategory.SelectedValue != null)
            {
                _currentProduct.CategoryId = (int)CbCategory.SelectedValue;
            }
        }

        private void BtnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp"
            };

            if (dialog.ShowDialog() != true) return;

            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images");
            Directory.CreateDirectory(imagesFolder);

            string newFileName = $"{Guid.NewGuid()}{Path.GetExtension(dialog.FileName)}";
            string destinationPath = Path.Combine(imagesFolder, newFileName);
            File.Copy(dialog.FileName, destinationPath, true);

            if (string.IsNullOrWhiteSpace(_currentProduct.ImageUrl))
                _currentProduct.ImageUrl = newFileName;
            else
                _currentProduct.ImageUrl += ";" + newFileName;

            RefreshImages();
        }

        private void RefreshImages()
        {
            _images = string.IsNullOrWhiteSpace(_currentProduct.ImageUrl)
                ? new List<string>()
                : _currentProduct.ImageUrl.Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            _currentIndex = 0;
            ShowImage();
        }

        private void BtnPrevImage_Click(object sender, RoutedEventArgs e)
        {
            if (_images.Count == 0) return;
            _currentIndex = (_currentIndex - 1 + _images.Count) % _images.Count;
            ShowImage();
        }

        private void BtnNextImage_Click(object sender, RoutedEventArgs e)
        {
            if (_images.Count == 0) return;
            _currentIndex = (_currentIndex + 1) % _images.Count;
            ShowImage();
        }

        private void ShowImage()
        {
            ImgPreview.Source = null;
            if (_images.Count == 0) return;

            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", _images[_currentIndex]);
            if (File.Exists(imagePath))
                ImgPreview.Source = new BitmapImage(new Uri(imagePath));
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentProduct.Name))
            {
                MessageBox.Show("Введите название товара.");
                return;
            }

            if (!decimal.TryParse(TbPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedPrice))
            {
                MessageBox.Show("Введите корректную цену.");
                return;
            }

            _currentProduct.Price = parsedPrice;

            if (CbCategory.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию.");
                return;
            }

            if (_productId == 0)
                AppConnect.model01.Flowers.Add(_currentProduct);

            MessageBox.Show("Сохранение выполнено.");
            AppFrame.frmMain.Navigate(new PageTask());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.Navigate(new PageTask());
        }
    }
}