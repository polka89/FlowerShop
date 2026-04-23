using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FlowerShop.ApplicationData
{
    public static class OrderStorage
    {
        private static string _ordersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "orders.json");
        private static readonly object _lock = new object();

        public static List<OrderData> GetAllOrders()
        {
            lock (_lock)
            {
                if (!File.Exists(_ordersFilePath))
                {
                    return new List<OrderData>();
                }

                string json = File.ReadAllText(_ordersFilePath);
                return JsonSerializer.Deserialize<List<OrderData>>(json) ?? new List<OrderData>();
            }
        }

        public static void SaveOrder(OrderData order)
        {
            lock (_lock)
            {
                var orders = GetAllOrders();
                order.Id = orders.Count > 0 ? orders.Max(o => o.Id) + 1 : 1;
                order.OrderDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                orders.Add(order);

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(orders, options);
                File.WriteAllText(_ordersFilePath, json);
            }
        }

        public static List<OrderData> GetOrdersByUser(int userId)
        {
            var orders = GetAllOrders();
            return orders.Where(o => o.UserId == userId).ToList();
        }

        public static List<OrderData> GetAllOrdersList()
        {
            return GetAllOrders();
        }
    }

    public class OrderData
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string OrderDate { get; set; }
        public string ItemsCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DeliveryPrice { get; set; }
        public decimal FinalTotal { get; set; }
        public string DeliveryMethod { get; set; }
        public string DeliveryAddress { get; set; }
        public List<OrderItemData> Items { get; set; }
    }

    public class OrderItemData
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }
}