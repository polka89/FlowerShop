using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FlowerShop.ApplicationData;

namespace FlowerShop.Data
{
    public class DatabaseHelper
    {
        private static ShopModel _shop = new ShopModel();

        public List<FlowerProduct> GetAllProducts()
        {
            try
            {
                // Возвращаем свежие данные из ShopModel
                return _shop.Flowers.Where(f => f.IsAvailable).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return new List<FlowerProduct>();
            }
        }

        public List<dynamic> GetAllCategories()
        {
            try
            {
                return _shop.Categories.ToList<dynamic>();
            }
            catch
            {
                return new List<dynamic>();
            }
        }

        public void UpdateProductStock(int productId, int newStock)
        {
            try
            {
                var product = _shop.Flowers.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    product.Stock = newStock;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления склада: {ex.Message}",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public int CreateOrder(int userId, string deliveryMethod, string deliveryAddress, List<CartItemInfo> items, decimal totalAmount)
        {
            try
            {
                // Генерируем случайный номер заказа
                Random rnd = new Random();
                return rnd.Next(1000, 9999);
            }
            catch
            {
                return new Random().Next(1000, 9999);
            }
        }

        public void AddPayment(int orderId, decimal amount, string paymentMethod)
        {
            System.Diagnostics.Debug.WriteLine($"Платеж: Заказ {orderId}, Сумма {amount}, Способ {paymentMethod}");
        }
    }

    public class CartItemInfo
    {
        public int FlowerId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercent { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
    }
}