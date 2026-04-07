using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowerShop.ApplicationData  // ВАЖНО: именно такое пространство имен!
{
    public class ShopModel
    {
        public List<User> Users { get; } = new List<User>();
        public List<Role> Roles { get; } = new List<Role>();
        public List<FlowerProduct> Flowers { get; } = new List<FlowerProduct>();
        public List<CategoryModel> Categories { get; } = new List<CategoryModel>();
        public List<Bouquet> Bouquets { get; } = new List<Bouquet>();
        public List<OrderModel> Orders { get; } = new List<OrderModel>();
        public List<OrderItemModel> OrderItems { get; } = new List<OrderItemModel>();
        public List<PaymentModel> Payments { get; } = new List<PaymentModel>();
        public List<DeliveryMethod> DeliveryMethods { get; } = new List<DeliveryMethod>();
        public List<Supplier> Suppliers { get; } = new List<Supplier>();
        public List<InventoryMovement> InventoryMovements { get; } = new List<InventoryMovement>();
        public List<Discount> Discounts { get; } = new List<Discount>();
        public List<CartModel> Carts { get; } = new List<CartModel>();
        public List<CartItemModel> CartItems { get; } = new List<CartItemModel>();
        public List<ReviewModel> Reviews { get; } = new List<ReviewModel>();

        public ShopModel()
        {
            SeedDefaultData();
        }

        private void SeedDefaultData()
        {
            Roles.Add(new Role { Id = 1, Name = "admin", DisplayName = "Администратор" });
            Roles.Add(new Role { Id = 2, Name = "manager", DisplayName = "Менеджер" });
            Roles.Add(new Role { Id = 3, Name = "user", DisplayName = "Покупатель" });

            Categories.Add(new CategoryModel { Id = 1, Name = "Розы" });
            Categories.Add(new CategoryModel { Id = 2, Name = "Тюльпаны" });
            Categories.Add(new CategoryModel { Id = 3, Name = "Хризантемы" });

            Users.Add(new User
            {
                Id = 1,
                FullName = "Администратор",
                Login = "admin",
                Password = "admin",
                RoleId = 1,
                CreatedAt = DateTime.Now
            });
        }

        public User FindUser(string login, string password)
        {
            return Users.FirstOrDefault(u =>
                u.Login.Equals(login, StringComparison.OrdinalIgnoreCase) && u.Password == password);
        }
    }

    // Модели данных
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

    public class FlowerProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public bool IsAvailable { get; set; }
        public string ImageUrl { get; set; }
    }

    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Bouquet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Composition { get; set; }
    }

    public class OrderModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }

    public class OrderItemModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int FlowerId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class PaymentModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public string Method { get; set; }
    }

    public class DeliveryMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class InventoryMovement
    {
        public int Id { get; set; }
        public int FlowerId { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
    }

    public class Discount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Percent { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }

    public class CartModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CartItemModel
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int FlowerId { get; set; }
        public int Quantity { get; set; }
    }

    public class ReviewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FlowerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}