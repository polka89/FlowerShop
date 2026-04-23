using FlowerShop.ApplicationData;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlowerShop.Pages
{
    public partial class AddEditProduct : Page
    {
        private ShopModel _shop;
        private FlowerProduct _editingProduct;
        private bool _isEditMode = false;

        public AddEditProduct(ShopModel shop, FlowerProduct product = null)
        {
            InitializeComponent();
            _shop = DataStore.Shop; // Используем единое хранилище

            if (product != null)
            {
                _isEditMode = true;
                _editingProduct = product;
                TitleTextBlock.Text = "Редактирование товара";
                BtnAddEdit.Content = "Сохранить изменения";
                LoadProductData();
            }
            else
            {
                TitleTextBlock.Text = "Добавление товара";
                BtnAddEdit.Content = "Добавить товар";
            }

            LoadCategories();
        }

        private void LoadProductData()
        {
            if (_editingProduct != null)
            {
                TbName.Text = _editingProduct.Name;
                TbPrice.Text = _editingProduct.Price.ToString();
                TbStock.Text = _editingProduct.Stock.ToString();
                TbDiscountPercent.Text = _editingProduct.DiscountPercent.ToString();
                ChkIsAvailable.IsChecked = _editingProduct.IsAvailable;

                if (CategoryComboBox.ItemsSource != null)
                {
                    var category = CategoryComboBox.ItemsSource.Cast<dynamic>()
                        .FirstOrDefault(c => c.Id == _editingProduct.CategoryId);
                    if (category != null)
                    {
                        CategoryComboBox.SelectedItem = category;
                    }
                }
            }
        }

        private void LoadCategories()
        {
            try
            {
                CategoryComboBox.ItemsSource = _shop.Categories;
                CategoryComboBox.DisplayMemberPath = "Name";
                CategoryComboBox.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnAddEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

                if (!int.TryParse(TbStock.Text, out int stock) || stock < 0)
                {
                    MessageBox.Show("Введите корректное количество!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (CategoryComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите категорию!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int categoryId = (int)CategoryComboBox.SelectedValue;
                bool isAvailable = ChkIsAvailable.IsChecked ?? true;
                int discountPercent = 0;

                if (!string.IsNullOrWhiteSpace(TbDiscountPercent.Text))
                {
                    int.TryParse(TbDiscountPercent.Text, out discountPercent);
                }

                if (_isEditMode)
                {
                    _editingProduct.Name = TbName.Text;
                    _editingProduct.Price = price;
                    _editingProduct.Stock = stock;
                    _editingProduct.CategoryId = categoryId;
                    _editingProduct.IsAvailable = isAvailable;
                    _editingProduct.DiscountPercent = discountPercent;

                    MessageBox.Show("Товар успешно обновлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    int newId = _shop.Flowers.Count > 0 ? _shop.Flowers.Max(f => f.Id) + 1 : 1;

                    var newProduct = new FlowerProduct
                    {
                        Id = newId,
                        Name = TbName.Text,
                        Price = price,
                        Stock = stock,
                        CategoryId = categoryId,
                        IsAvailable = isAvailable,
                        DiscountPercent = discountPercent
                    };

                    _shop.Flowers.Add(newProduct);

                    MessageBox.Show("Товар успешно добавлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Уведомляем об обновлении данных
                DataStore.RefreshProducts();

                if (NavigationService != null && NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения товара: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}