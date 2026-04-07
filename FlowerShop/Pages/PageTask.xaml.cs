using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FlowerShop.ApplicationData;

namespace FlowerShop.Pages
{
    public partial class PageTask : Page
    {
        private List<FlowerProduct> _flowers;
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

            FlowersListView.ItemsSource = _flowers;

            CategoryComboBox.ItemsSource = _categories;
            CategoryComboBox.DisplayMemberPath = "Name";
            CategoryComboBox.SelectedValuePath = "Id";
            CategoryComboBox.SelectedIndex = -1;
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedValue != null)
            {
                int selectedCategoryId = (int)CategoryComboBox.SelectedValue;
                var filteredFlowers = AppConnect.model01.Flowers
                    .Where(f => f.CategoryId == selectedCategoryId)
                    .ToList();
                FlowersListView.ItemsSource = filteredFlowers;
            }
            else
            {
                FlowersListView.ItemsSource = _flowers;
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frmMain.Navigate(new AddRecip(0));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            FlowerProduct selectedFlower = FlowersListView.SelectedItem as FlowerProduct;
            if (selectedFlower != null)
            {
                AppFrame.frmMain.Navigate(new AddRecip(selectedFlower.Id));
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            FlowerProduct selectedFlower = FlowersListView.SelectedItem as FlowerProduct;
            if (selectedFlower != null)
            {
                var result = MessageBox.Show($"Удалить товар \"{selectedFlower.Name}\"?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    AppConnect.model01.Flowers.Remove(selectedFlower);
                    LoadData();
                    MessageBox.Show("Товар удален", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (AppFrame.frmMain.CanGoBack)
                AppFrame.frmMain.GoBack();
        }
    }
}