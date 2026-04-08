using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using FlowerShop.ApplicationData;

namespace FlowerShop.Pages
{
    public partial class PageTask : Page
    {
        private List<ApplicationData.FlowerProduct> _flowers;
        private List<CategoryModel> _categories;

        public PageTask()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            _flowers = AppConnect.model01.Flowers.ToList();
            _categories = AppConnect.model01.Categories.ToList();

            CategoriesPanel.ItemsSource = _categories;
            ProductsListBox.ItemsSource = _flowers;
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn?.Tag != null)
            {
                int categoryId = (int)btn.Tag;
                var filteredFlowers = AppConnect.model01.Flowers
                    .Where(f => f.CategoryId == categoryId)
                    .ToList();
                ProductsListBox.ItemsSource = filteredFlowers;
            }
        }

        private void BtnAllCategories_Click(object sender, RoutedEventArgs e)
        {
            ProductsListBox.ItemsSource = _flowers;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.Navigate(new AddRecip(0));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = ProductsListBox.SelectedItem as ApplicationData.FlowerProduct;
            if (selected != null)
            {
                AppFrame.frmMain.Navigate(new AddRecip(selected.Id));
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = ProductsListBox.SelectedItem as ApplicationData.FlowerProduct;
            if (selected != null)
            {
                var result = MessageBox.Show($"Удалить товар \"{selected.Name}\"?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        AppConnect.model01.Flowers.Remove(selected);
                        LoadData();
                        MessageBox.Show("Товар удален!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (AppFrame.frmMain.CanGoBack)
                AppFrame.frmMain.GoBack();
        }
    }

    public class AvailabilityTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool && (bool)value) ? "В наличии" : "Нет в наличии";
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
            return (value is bool && (bool)value) ? "#7A3E65" : "#CC3333";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // ГЛАВНЫЙ КОНВЕРТЕР ДЛЯ КАРТИНОК - ИСПРАВЛЕННЫЙ
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imageUrl = value as string;
            if (string.IsNullOrEmpty(imageUrl))
                return null;

            try
            {
                // Получаем базовую директорию (где лежит .exe файл)
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                // Пробуем разные пути
                string[] possiblePaths = {
                    // Путь 1: папка images в корне с программой
                    System.IO.Path.Combine(baseDir, "images", imageUrl),
                    // Путь 2: папка Resources/Images
                    System.IO.Path.Combine(baseDir, "Resources", "Images", imageUrl),
                    // Путь 3: прямо в корне с программой
                    System.IO.Path.Combine(baseDir, imageUrl),
                    // Путь 4: папка images на уровень выше (для отладки)
                    System.IO.Path.Combine(baseDir, "..", "..", "images", imageUrl),
                };

                foreach (var path in possiblePaths)
                {
                    // Очищаем путь от возможных ".."
                    string fullPath = System.IO.Path.GetFullPath(path);

                    if (System.IO.File.Exists(fullPath))
                    {
                        var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                        bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                        bitmap.EndInit();

                        // Отладка - пишем в Output окно Visual Studio
                        System.Diagnostics.Debug.WriteLine($"✅ Картинка загружена: {fullPath}");

                        return bitmap;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Файл не найден: {fullPath}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"⚠️ Картинка НЕ НАЙДЕНА: {imageUrl}");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки картинки: {ex.Message}");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}