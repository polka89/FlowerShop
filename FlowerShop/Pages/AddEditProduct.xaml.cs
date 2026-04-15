using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FlowerShop.ApplicationData;

namespace FlowerShop.Pages
{
    public partial class AddEditProduct : Page
    {
        private ShopModel _shop;
        private FlowerProduct _editingProduct;
        private bool _isEditMode;

        public AddEditProduct(ShopModel shop, FlowerProduct product = null)
        {
            InitializeComponent();
            _shop = shop;
            _editingProduct = product;
            _isEditMode = product != null;

            LoadCategories();

            if (_isEditMode)
            {
                TitleText.Text = "✏️ Редактирование товара";
                LoadProductData();
            }
            else
            {
                TitleText.Text = "➕ Добавление товара";
            }
        }

        private void LoadCategories()
        {
            CategoryBox.ItemsSource = _shop.Categories;
            if (CategoryBox.Items.Count > 0)
                CategoryBox.SelectedIndex = 0;
        }

        private void LoadProductData()
        {
            if (_editingProduct != null)
            {
                NameBox.Text = _editingProduct.Name;
                PriceBox.Text = _editingProduct.Price.ToString();
                ImageBox.Text = _editingProduct.ImageUrl;
                RatingBox.Text = _editingProduct.Rating.ToString();
                DiscountBox.Text = _editingProduct.DiscountPercent.ToString();
                AvailableBox.IsChecked = _editingProduct.IsAvailable;

                // Выбираем категорию
                for (int i = 0; i < CategoryBox.Items.Count; i++)
                {
                    var cat = CategoryBox.Items[i] as CategoryModel;
                    if (cat != null && cat.Id == _editingProduct.CategoryId)
                    {
                        CategoryBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка полей
                if (string.IsNullOrWhiteSpace(NameBox.Text))
                {
                    MessageBox.Show("Введите название товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    NameBox.Focus();
                    return;
                }

                if (!decimal.TryParse(PriceBox.Text, out decimal price) || price <= 0)
                {
                    MessageBox.Show("Введите корректную цену!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    PriceBox.Focus();
                    return;
                }

                if (!decimal.TryParse(RatingBox.Text, out decimal rating) || rating < 0 || rating > 5)
                {
                    rating = 4.5m;
                }

                if (!int.TryParse(DiscountBox.Text, out int discount) || discount < 0 || discount > 100)
                {
                    discount = 0;
                }

                var selectedCategory = CategoryBox.SelectedItem as CategoryModel;
                if (selectedCategory == null)
                {
                    MessageBox.Show("Выберите категорию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_isEditMode)
                {
                    // РЕДАКТИРОВАНИЕ
                    _editingProduct.Name = NameBox.Text;
                    _editingProduct.Price = price;
                    _editingProduct.CategoryId = selectedCategory.Id;
                    _editingProduct.ImageUrl = string.IsNullOrWhiteSpace(ImageBox.Text) ? "default.png" : ImageBox.Text;
                    _editingProduct.Rating = rating;
                    _editingProduct.DiscountPercent = discount;
                    _editingProduct.IsAvailable = AvailableBox.IsChecked ?? true;

                    bool result = _shop.UpdateFlower(_editingProduct);
                    if (result)
                    {
                        MessageBox.Show("Товар успешно обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    // ДОБАВЛЕНИЕ
                    var newProduct = new FlowerProduct
                    {
                        Name = NameBox.Text,
                        Price = price,
                        CategoryId = selectedCategory.Id,
                        ImageUrl = string.IsNullOrWhiteSpace(ImageBox.Text) ? "default.png" : ImageBox.Text,
                        Rating = rating,
                        DiscountPercent = discount,
                        IsAvailable = AvailableBox.IsChecked ?? true
                    };
                    _shop.AddFlower(newProduct);
                    MessageBox.Show("Товар успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Возвращаемся на страницу каталога
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}