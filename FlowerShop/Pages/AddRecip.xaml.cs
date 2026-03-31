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
        private flowers _currentProduct;
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
            CbCategory.ItemsSource = AppConnect.model01.categories.OrderBy(c => c.name).ToList();

            if (_productId == 0)
            {
                _currentProduct = new flowers
                {
                    created_at = DateTime.Now,
                    is_available = true,
                    stock_quantity = 1
                };
            }
            else
            {
                _currentProduct = AppConnect.model01.flowers.FirstOrDefault(f => f.id == _productId);
                if (_currentProduct == null)
                {
                    MessageBox.Show("Товар не найден.");
                    AppFrame.frmMain.Navigate(new PageTask());
                    return;
                }
            }

            DataContext = _currentProduct;
            CbCategory.SelectedValue = _currentProduct.category_id;
            RefreshImages();
        }

        private void CbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_currentProduct != null && CbCategory.SelectedValue != null)
            {
                _currentProduct.category_id = (int)CbCategory.SelectedValue;
            }
        }

        private void BtnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images");
            Directory.CreateDirectory(imagesFolder);

            string newFileName = $"{Guid.NewGuid()}{Path.GetExtension(dialog.FileName)}";
            string destinationPath = Path.Combine(imagesFolder, newFileName);
            File.Copy(dialog.FileName, destinationPath, true);

            if (string.IsNullOrWhiteSpace(_currentProduct.image_url))
            {
                _currentProduct.image_url = newFileName;
            }
            else
            {
                _currentProduct.image_url += ";" + newFileName;
            }

            RefreshImages();
        }

        private void RefreshImages()
        {
            _images = string.IsNullOrWhiteSpace(_currentProduct.image_url)
                ? new List<string>()
                : _currentProduct.image_url.Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

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
            if (_images.Count == 0)
            {
                return;
            }

            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", _images[_currentIndex]);
            if (File.Exists(imagePath))
            {
                ImgPreview.Source = new BitmapImage(new Uri(imagePath));
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentProduct.name))
            {
                MessageBox.Show("Введите название товара.");
                return;
            }

            if (!decimal.TryParse(TbPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedPrice) &&
                !decimal.TryParse(TbPrice.Text, out parsedPrice))
            {
                MessageBox.Show("Введите корректную цену.");
                return;
            }

            _currentProduct.price = parsedPrice;
            _currentProduct.updated_at = DateTime.Now;

            if (CbCategory.SelectedItem is categories selectedCategory)
            {
                var categoryInDb = AppConnect.model01.categories.FirstOrDefault(c => c.id == selectedCategory.id);
                if (categoryInDb == null)
                {
                    MessageBox.Show("Категория не найдена.");
                    return;
                }

                _currentProduct.category_id = categoryInDb.id;
            }
            else
            {
                MessageBox.Show("Выберите категорию.");
                return;
            }

            if (_productId == 0)
            {
                AppConnect.model01.flowers.Add(_currentProduct);
            }

            AppConnect.model01.SaveChanges();
            MessageBox.Show("Сохранение выполнено.");
            AppFrame.frmMain.Navigate(new PageTask());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.Navigate(new PageTask());
        }
    }
}
