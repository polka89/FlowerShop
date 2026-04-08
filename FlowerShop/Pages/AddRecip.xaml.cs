using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        private ApplicationData.FlowerProduct _currentProduct; // Указываем полное имя
        private List<string> _images = new List<string>();
        private int _currentIndex;

        public AddRecip(int id = 0)
        {
            InitializeComponent();
            _productId = id;
            InitializeData();
        }

        private void InitializeData()
        {
            // Загрузка категорий
            CbCategory.ItemsSource = AppConnect.model01.Categories.OrderBy(c => c.Name).ToList();

            if (_productId == 0)
            {
                // Режим добавления нового товара
                TitleText.Text = "Добавление новой позиции в каталог 🌹";
                _currentProduct = new ApplicationData.FlowerProduct // Указываем полное имя
                {
                    Id = GetNextId(),
                    IsAvailable = true,
                    Price = 0,
                    Name = "",
                    ImageUrl = ""
                };
                CbIsAvailable.IsChecked = true;
            }
            else
            {
                // Режим редактирования существующего товара
                TitleText.Text = "Редактирование товара ✏️";
                _currentProduct = AppConnect.model01.Flowers.FirstOrDefault(f => f.Id == _productId);

                if (_currentProduct == null)
                {
                    MessageBox.Show("Товар не найден!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    AppFrame.frmMain.Navigate(new PageTask());
                    return;
                }

                CbIsAvailable.IsChecked = _currentProduct.IsAvailable;
            }

            // Заполнение полей
            TbName.Text = _currentProduct.Name;
            TbPrice.Text = _currentProduct.Price.ToString();
            TbImageUrl.Text = _currentProduct.ImageUrl;

            if (_currentProduct.CategoryId > 0)
            {
                CbCategory.SelectedValue = _currentProduct.CategoryId;
            }

            RefreshImages();
        }

        private int GetNextId()
        {
            if (AppConnect.model01.Flowers.Count == 0)
                return 1;
            return AppConnect.model01.Flowers.Max(f => f.Id) + 1;
        }

        private void CbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbCategory.SelectedValue != null && _currentProduct != null)
            {
                _currentProduct.CategoryId = (int)CbCategory.SelectedValue;
            }
        }

        private void BtnUploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
                Title = "Выберите изображение"
            };

            if (dialog.ShowDialog() == true)
            {
                string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images");
                Directory.CreateDirectory(imagesFolder);

                string newFileName = $"{Guid.NewGuid()}{Path.GetExtension(dialog.FileName)}";
                string destinationPath = Path.Combine(imagesFolder, newFileName);
                File.Copy(dialog.FileName, destinationPath, true);

                if (string.IsNullOrWhiteSpace(_currentProduct.ImageUrl))
                    _currentProduct.ImageUrl = newFileName;
                else
                    _currentProduct.ImageUrl += ";" + newFileName;

                TbImageUrl.Text = _currentProduct.ImageUrl;
                RefreshImages();

                MessageBox.Show("Изображение загружено!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RefreshImages()
        {
            _images = string.IsNullOrWhiteSpace(_currentProduct.ImageUrl)
                ? new List<string>()
                : _currentProduct.ImageUrl.Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            _currentIndex = 0;
            ShowImage();
            TbImageCounter.Text = _images.Count == 0 ? "Нет изображений" : $"Изображение {_currentIndex + 1} из {_images.Count}";
        }

        private void BtnPrevImage_Click(object sender, RoutedEventArgs e)
        {
            if (_images.Count == 0) return;
            _currentIndex = (_currentIndex - 1 + _images.Count) % _images.Count;
            ShowImage();
            TbImageCounter.Text = $"Изображение {_currentIndex + 1} из {_images.Count}";
        }

        private void BtnNextImage_Click(object sender, RoutedEventArgs e)
        {
            if (_images.Count == 0) return;
            _currentIndex = (_currentIndex + 1) % _images.Count;
            ShowImage();
            TbImageCounter.Text = $"Изображение {_currentIndex + 1} из {_images.Count}";
        }

        private void ShowImage()
        {
            ImgPreview.Source = null;
            if (_images.Count == 0) return;

            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", _images[_currentIndex]);
            if (File.Exists(imagePath))
            {
                ImgPreview.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(TbName.Text))
                {
                    MessageBox.Show("Введите название товара!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(TbPrice.Text, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Введите корректную цену!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (CbCategory.SelectedItem == null)
                {
                    MessageBox.Show("Выберите категорию!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Сохранение данных
                _currentProduct.Name = TbName.Text;
                _currentProduct.Price = price;
                _currentProduct.IsAvailable = CbIsAvailable.IsChecked == true;
                _currentProduct.ImageUrl = TbImageUrl.Text;

                if (_productId == 0)
                {
                    // Добавление нового товара
                    AppConnect.model01.Flowers.Add(_currentProduct);
                    MessageBox.Show("Товар успешно добавлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Обновление существующего товара
                    MessageBox.Show("Товар успешно обновлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Возврат к списку товаров
                AppFrame.frmMain.Navigate(new PageTask());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.Navigate(new PageTask());
        }
    }
}