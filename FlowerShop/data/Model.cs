using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowerShop.ApplicationData
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
            // Роли
            Roles.Add(new Role { Id = 1, Name = "admin", DisplayName = "Администратор" });
            Roles.Add(new Role { Id = 2, Name = "manager", DisplayName = "Менеджер" });
            Roles.Add(new Role { Id = 3, Name = "user", DisplayName = "Покупатель" });

            // Категории
            Categories.Add(new CategoryModel { Id = 1, Name = "Розы" });
            Categories.Add(new CategoryModel { Id = 2, Name = "Тюльпаны" });
            Categories.Add(new CategoryModel { Id = 3, Name = "Хризантемы" });
            Categories.Add(new CategoryModel { Id = 4, Name = "Пионы" });
            Categories.Add(new CategoryModel { Id = 5, Name = "Готовые букеты" });
            Categories.Add(new CategoryModel { Id = 6, Name = "Альстромерии" });
            Categories.Add(new CategoryModel { Id = 7, Name = "Каллы" });
            Categories.Add(new CategoryModel { Id = 8, Name = "Ромашки" });
            Categories.Add(new CategoryModel { Id = 9, Name = "Лилии" });
            Categories.Add(new CategoryModel { Id = 10, Name = "Гвоздики" });
            Categories.Add(new CategoryModel { Id = 11, Name = "Лаванда" });
            Categories.Add(new CategoryModel { Id = 12, Name = "Ирисы" });
            Categories.Add(new CategoryModel { Id = 13, Name = "Орхидеи" });
            Categories.Add(new CategoryModel { Id = 14, Name = "Лизиантусы" });
            Categories.Add(new CategoryModel { Id = 15, Name = "Комнатные растения" });
            Categories.Add(new CategoryModel { Id = 16, Name = "Гипсофилы" });

            // ==================== ВСЕ ТОВАРЫ С ФОТОГРАФИЯМИ ====================

            // ========== 1. РОЗЫ (категория Id = 1) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 1,
                Name = "Красная роза",
                Price = 150,
                CategoryId = 1,
                IsAvailable = true,
                ImageUrl = "красная роза.jpg",
                Rating = 4.9m,
                DiscountPercent = 0
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 2,
                Name = "Белая роза",
                Price = 160,
                CategoryId = 1,
                IsAvailable = true,
                ImageUrl = "converto.in_белая роза (1).png",
                Rating = 4.8m,
                DiscountPercent = 0
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 3,
                Name = "Роза пионовидная",
                Price = 200,
                CategoryId = 1,
                IsAvailable = true,
                ImageUrl = "convertio.in_Пионовидная роза.png",
                Rating = 4.9m,
                DiscountPercent = 10
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 4,
                Name = "Кустовая роза",
                Price = 180,
                CategoryId = 1,
                IsAvailable = true,
                ImageUrl = "converto.in_Кустовая роза (1).png",
                Rating = 4.7m,
                DiscountPercent = 0
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 5,
                Name = "Роза Эквадор",
                Price = 250,
                CategoryId = 1,
                IsAvailable = true,
                ImageUrl = "роза эквадор.jpg",
                Rating = 5.0m,
                DiscountPercent = 15
            });

            // ========== 2. ТЮЛЬПАНЫ (категория Id = 2) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 6,
                Name = "Красный тюльпан",
                Price = 120,
                CategoryId = 2,
                IsAvailable = true,
                ImageUrl = "красный тюльпан.jpg",
                Rating = 4.6m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 7,
                Name = "Желтый тюльпан",
                Price = 120,
                CategoryId = 2,
                IsAvailable = true,
                ImageUrl = "converto.in_Желтый тюльпан (1).png",
                Rating = 4.5m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 8,
                Name = "Розовый тюльпан",
                Price = 130,
                CategoryId = 2,
                IsAvailable = true,
                ImageUrl = "convertio.in_Розовый тюльпан.png",
                Rating = 4.7m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 9,
                Name = "Кьюминс тюльпан",
                Price = 140,
                CategoryId = 2,
                IsAvailable = true,
                ImageUrl = "converto.in_Кьюминс тюльпан.png",
                Rating = 4.8m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 10,
                Name = "Белый тюльпан",
                Price = 120,
                CategoryId = 2,
                IsAvailable = true,
                ImageUrl = "converto.in_Белый тюльпан (1).png",
                Rating = 4.6m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 11,
                Name = "Пионовидный тюльпан",
                Price = 120,
                CategoryId = 2,
                IsAvailable = true,
                ImageUrl = "convertio.in_Пионовидный тюльпан (1).png",
                Rating = 4.9m
            });

            // ========== 3. ХРИЗАНТЕМЫ (категория Id = 3) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 12,
                Name = "Кустовая хризантема",
                Price = 180,
                CategoryId = 3,
                IsAvailable = true,
                ImageUrl = "converto.in_Кустовая хризантема (1).png",
                Rating = 4.7m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 13,
                Name = "Крупноцветковая хризантема",
                Price = 220,
                CategoryId = 3,
                IsAvailable = true,
                ImageUrl = "converto.in_Крупноцветковая хризантема.png",
                Rating = 4.8m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 14,
                Name = "Хризантема мультифлора",
                Price = 200,
                CategoryId = 3,
                IsAvailable = true,
                ImageUrl = "convertio.in_Хризантема мультифлора.png",
                Rating = 4.9m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 15,
                Name = "Розовая хризантема",
                Price = 180,
                CategoryId = 3,
                IsAvailable = true,
                ImageUrl = "convertio.in_Розовая хризантема (1).png",
                Rating = 4.7m
            });

            // ========== 4. ПИОНЫ (категория Id = 4) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 16,
                Name = "Розовый пион",
                Price = 250,
                CategoryId = 4,
                IsAvailable = true,
                ImageUrl = "convertio.in_Розовый пион (1).png",
                Rating = 4.9m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 17,
                Name = "Белый пион",
                Price = 260,
                CategoryId = 4,
                IsAvailable = true,
                ImageUrl = "converto.in_Белый пион.png",
                Rating = 4.8m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 18,
                Name = "Бордовый пион",
                Price = 280,
                CategoryId = 4,
                IsAvailable = true,
                ImageUrl = "converto.in_Бордовый пион (1).png",
                Rating = 4.9m,
                DiscountPercent = 5
            });

            // ========== 5. ГОТОВЫЕ БУКЕТЫ (категория Id = 5) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 19,
                Name = "Букет нежных цветов эустома и розы",
                Price = 3550,
                CategoryId = 5,
                IsAvailable = true,
                ImageUrl = "converto.in_Букет нежных цветов эустома и розы.png",
                Rating = 4.9m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 20,
                Name = "Букет с Ирисами и Лизиантусом",
                Price = 4200,
                CategoryId = 5,
                IsAvailable = true,
                ImageUrl = "converto.in_Букет с Ирисами и Лизиантусом.png",
                Rating = 4.8m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 21,
                Name = "Букет полевых цветов",
                Price = 800,
                CategoryId = 5,
                IsAvailable = true,
                ImageUrl = "converto.in_Букет полевых цветов.png",
                Rating = 4.7m,
                DiscountPercent = 10
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 22,
                Name = "Букет в нежном розовом цвете",
                Price = 6000,
                CategoryId = 5,
                IsAvailable = true,
                ImageUrl = "converto.in_Букет в нежном розовом цвете.png",
                Rating = 5.0m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 23,
                Name = "Северное сияние",
                Price = 4600,
                CategoryId = 5,
                IsAvailable = true,
                ImageUrl = "converto.in_Северное сияние Букет из экзотичной орхидеи с альстромерией.png",
                Rating = 4.9m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 24,
                Name = "Букет из 101 пионовидной розы",
                Price = 50600,
                CategoryId = 5,
                IsAvailable = true,
                ImageUrl = "converto.in_Букет из 101 пионовидной розы (1).png",
                Rating = 5.0m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 25,
                Name = "Моно букет кустовых роз",
                Price = 3666,
                CategoryId = 5,
                IsAvailable = true,
                ImageUrl = "converto.in_Моно букет кустовых роз.png",
                Rating = 4.8m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 26,
                Name = "Букет из альстромерий",
                Price = 2300,
                CategoryId = 5,
                IsAvailable = true,
                ImageUrl = "converto.in_Букет из альстромерий.png",
                Rating = 4.7m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 27,
                Name = "Букет из кустовых роз, альстромерий и лилии",
                Price = 10500,
                CategoryId = 5,
                IsAvailable = true,
                ImageUrl = "converto.in_Букет из кустовых роз, альстромерий и лилии.png",
                Rating = 4.9m
            });

            // ========== 6. АЛЬСТРОМЕРИИ (категория Id = 6) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 28,
                Name = "Альстромерия розовая",
                Price = 130,
                CategoryId = 6,
                IsAvailable = true,
                ImageUrl = "converto.in_Альстромерия розовая.png",
                Rating = 4.7m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 29,
                Name = "Альстромерия фиолетовая",
                Price = 130,
                CategoryId = 6,
                IsAvailable = true,
                ImageUrl = "converto.in_Альстромерия фиолетовая.png",
                Rating = 4.8m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 30,
                Name = "Альстромерия белая",
                Price = 140,
                CategoryId = 6,
                IsAvailable = true,
                ImageUrl = "converto.in_Альстромерия белая.png",
                Rating = 4.7m
            });

            // ========== 7. КАЛЛЫ (категория Id = 7) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 31,
                Name = "Калла белая",
                Price = 300,
                CategoryId = 7,
                IsAvailable = true,
                ImageUrl = "converto.in_Калла белая.png",
                Rating = 4.9m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 32,
                Name = "Калла бордовая",
                Price = 320,
                CategoryId = 7,
                IsAvailable = true,
                ImageUrl = "converto.in_Калла бордовая.png",
                Rating = 4.8m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 33,
                Name = "Калла розовая",
                Price = 300,
                CategoryId = 7,
                IsAvailable = true,
                ImageUrl = "converto.in_Калла розовая.png",
                Rating = 4.9m
            });

            // ========== 8. РОМАШКИ (категория Id = 8) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 34,
                Name = "Ромашка аптечная",
                Price = 80,
                CategoryId = 8,
                IsAvailable = true,
                ImageUrl = "convertio.in_Ромашка аптечная.png",
                Rating = 4.5m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 35,
                Name = "Ромашка крупная",
                Price = 100,
                CategoryId = 8,
                IsAvailable = true,
                ImageUrl = "convertio.in_Ромашка крупная.png",
                Rating = 4.6m
            });

            // ========== 9. ЛИЛИИ (категория Id = 9) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 36,
                Name = "Лилия белая",
                Price = 1500,
                CategoryId = 9,
                IsAvailable = true,
                ImageUrl = "Лилия белая.jpg",
                Rating = 4.9m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 37,
                Name = "Лилия голубая",
                Price = 1000,
                CategoryId = 9,
                IsAvailable = true,
                ImageUrl = "Голубая лилия.jpg",
                Rating = 4.8m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 38,
                Name = "Лилия розовая",
                Price = 2100,
                CategoryId = 9,
                IsAvailable = true,
                ImageUrl = "Розовая лилия.jpg",
                Rating = 4.9m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 39,
                Name = "Лилия тигровая",
                Price = 1300,
                CategoryId = 9,
                IsAvailable = true,
                ImageUrl = "converto.in_Лилия тигровая.png",
                Rating = 4.8m
            });

            // ========== 10. ГВОЗДИКИ (категория Id = 10) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 40,
                Name = "Гвоздика красная",
                Price = 100,
                CategoryId = 10,
                IsAvailable = true,
                ImageUrl = "converto.in_Гвоздика красная.png",
                Rating = 4.6m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 41,
                Name = "Гвоздика белая",
                Price = 100,
                CategoryId = 10,
                IsAvailable = true,
                ImageUrl = "converto.in_Гвоздика белая.png",
                Rating = 4.5m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 42,
                Name = "Гвоздика розовая",
                Price = 110,
                CategoryId = 10,
                IsAvailable = true,
                ImageUrl = "converto.in_Гвоздика розовая.png",
                Rating = 4.7m
            });

            // ========== 11. ЛАВАНДА (категория Id = 11) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 43,
                Name = "Лаванда узколистная",
                Price = 120,
                CategoryId = 11,
                IsAvailable = true,
                ImageUrl = "converto.in_Лаванда узколистная.png",
                Rating = 4.8m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 44,
                Name = "Лаванда французская",
                Price = 140,
                CategoryId = 11,
                IsAvailable = true,
                ImageUrl = "converto.in_Лаванда французская.png",
                Rating = 4.9m
            });

            // ========== 12. ИРИСЫ (категория Id = 12) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 45,
                Name = "Ирис синий",
                Price = 150,
                CategoryId = 12,
                IsAvailable = true,
                ImageUrl = "converto.in_Ирис синий.png",
                Rating = 4.7m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 46,
                Name = "Ирис желтый",
                Price = 150,
                CategoryId = 12,
                IsAvailable = true,
                ImageUrl = "converto.in_Ирис желтый.png",
                Rating = 4.6m
            });

            // ========== 13. ОРХИДЕИ (категория Id = 13) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 47,
                Name = "Орхидея фаленопсис белая",
                Price = 500,
                CategoryId = 13,
                IsAvailable = true,
                ImageUrl = "converto.in_Орхидея фаленопсис белая.png",
                Rating = 4.9m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 48,
                Name = "Орхидея фаленопсис розовая",
                Price = 550,
                CategoryId = 13,
                IsAvailable = true,
                ImageUrl = "converto.in_Орхидея фаленопсис розовая.png",
                Rating = 4.9m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 49,
                Name = "Орхидея цимбидиум",
                Price = 600,
                CategoryId = 13,
                IsAvailable = true,
                ImageUrl = "convertio.in_Орхидея цимбидиум.png",
                Rating = 4.8m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 50,
                Name = "Орхидея дендробиум",
                Price = 520,
                CategoryId = 13,
                IsAvailable = true,
                ImageUrl = "converto.in_Орхидея дендробиум.png",
                Rating = 4.9m
            });

            // ========== 14. ЛИЗИАНТУСЫ (категория Id = 14) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 51,
                Name = "Лизиантус белый",
                Price = 180,
                CategoryId = 14,
                IsAvailable = true,
                ImageUrl = "converto.in_Лизиантус белый.png",
                Rating = 4.8m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 52,
                Name = "Лизиантус розовый",
                Price = 190,
                CategoryId = 14,
                IsAvailable = true,
                ImageUrl = "converto.in_Лизиантус розовый.png",
                Rating = 4.9m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 53,
                Name = "Лизиантус фиолетовый",
                Price = 190,
                CategoryId = 14,
                IsAvailable = true,
                ImageUrl = "converto.in_Лизиантус фиолетовый.png",
                Rating = 4.8m
            });

            // ========== 15. КОМНАТНЫЕ РАСТЕНИЯ (категория Id = 15) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 54,
                Name = "Спатифиллум",
                Price = 400,
                CategoryId = 15,
                IsAvailable = true,
                ImageUrl = "convertio.in_Спатифиллум.png",
                Rating = 4.8m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 55,
                Name = "Антуриум",
                Price = 450,
                CategoryId = 15,
                IsAvailable = true,
                ImageUrl = "converto.in_Антуриум (1).png",
                Rating = 4.9m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 56,
                Name = "Фикус Бенджамина",
                Price = 800,
                CategoryId = 15,
                IsAvailable = true,
                ImageUrl = "convertio.in_Фикус Бенджамина (1).png",
                Rating = 4.7m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 57,
                Name = "Монстера",
                Price = 1200,
                CategoryId = 15,
                IsAvailable = true,
                ImageUrl = "converto.in_Монстера (1).png",
                Rating = 4.9m
            });

            // ========== 16. ГИПСОФИЛЫ (категория Id = 16) ==========
            Flowers.Add(new FlowerProduct
            {
                Id = 58,
                Name = "Гипсофила метельчатая",
                Price = 90,
                CategoryId = 16,
                IsAvailable = true,
                ImageUrl = "converto.in_Гипсофия метельчатая.png",
                Rating = 4.5m
            });

            Flowers.Add(new FlowerProduct
            {
                Id = 59,
                Name = "Гипсофила изящная",
                Price = 100,
                CategoryId = 16,
                IsAvailable = true,
                ImageUrl = "converto.in_Гипсофия изящная.png",
                Rating = 4.6m
            });

            // Админ
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

    // ==================== МОДЕЛИ ДАННЫХ ====================

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
        public decimal Rating { get; set; } = 4.85m;
        public int DiscountPercent { get; set; } = 0;

        public bool HasDiscount => DiscountPercent > 0;
        public decimal OldPrice => HasDiscount ? Price * 100 / (100 - DiscountPercent) : Price;
        public string DisplayPrice => $"{Price:0} ₽";
        public string RatingText => Rating.ToString("0.0");

        public string InstallmentText
        {
            get
            {
                if (Price == 0) return "0 ₽ × 4 платежа";
                int installmentPrice = (int)(Price / 4);
                return $"{installmentPrice} ₽ × 4 платежа";
            }
        }
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