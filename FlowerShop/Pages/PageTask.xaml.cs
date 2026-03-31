using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FlowerShop.ApplicationData;

namespace FlowerShop.Pages
{
    public partial class PageTask : Page
    {
        private List<flowers> _products = new List<flowers>();
        private readonly HashSet<int> _favorites = new HashSet<int>();

        public PageTask()
        {
            InitializeComponent();
            InitializePage();
        }

        private void InitializePage()
        {
            InitializeFilters();
            LoadProducts();
            UpdateCounter();
        }

        private void InitializeFilters()
        {
            var categories = AppConnect.model01.categories.OrderBy(c => c.name).ToList();
            categories.Insert(0, new categories { id = 0, name = "Все категории" });
            CbCategory.ItemsSource = categories;
            CbCategory.DisplayMemberPath = "name";
            CbCategory.SelectedValuePath = "id";
            CbCategory.SelectedIndex = 0;
        }

        private void LoadProducts()
        {
            var query = AppConnect.model01.flowers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(TbSearch.Text))
            {
                string search = TbSearch.Text.Trim().ToLower();
                query = query.Where(p => p.name.ToLower().Contains(search));
            }

            if (CbCategory.SelectedItem is categories category && category.id != 0)
            {
                query = query.Where(p => p.category_id == category.id);
            }

            string sortTag = (CbSort.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "none";
            switch (sortTag)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.price);
                    break;
                case "new":
                    query = query.OrderByDescending(p => p.created_at);
                    break;
            }

            _products = query.ToList();
            LvProducts.ItemsSource = _products;
            IcProducts.ItemsSource = _products;
            UpdateCounter();
        }

        private void UpdateCounter()
        {
            TbCounter.Text = $"Найдено товаров: {_products.Count}";
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadProducts();
        private void CbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadProducts();
        private void CbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadProducts();

        private void BtnListView_Click(object sender, RoutedEventArgs e)
        {
            LvProducts.Visibility = Visibility.Visible;
            SvTiles.Visibility = Visibility.Collapsed;
        }

        private void BtnTileView_Click(object sender, RoutedEventArgs e)
        {
            LvProducts.Visibility = Visibility.Collapsed;
            SvTiles.Visibility = Visibility.Visible;
        }

        private void BtnFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag?.ToString(), out int id))
            {
                if (_favorites.Contains(id))
                {
                    _favorites.Remove(id);
                    MessageBox.Show("Удалено из избранного");
                }
                else
                {
                    _favorites.Add(id);
                    MessageBox.Show("Добавлено в избранное");
                }
            }
        }

        private void LvProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LvProducts.SelectedItem is flowers selected)
            {
                AppFrame.frmMain.Navigate(new AddRecip(selected.id));
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AppSession.CurrentRole != "admin")
            {
                MessageBox.Show("Добавление доступно только администратору.");
                return;
            }

            AppFrame.frmMain.Navigate(new AddRecip(0));
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Image image && image.Tag is string imageName && !string.IsNullOrWhiteSpace(imageName))
            {
                string firstImage = imageName.Split(';').FirstOrDefault();
                if (string.IsNullOrWhiteSpace(firstImage))
                {
                    return;
                }

                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", firstImage);
                if (File.Exists(imagePath))
                {
                    image.Source = new BitmapImage(new Uri(imagePath));
                }
            }
        }
    }
}
