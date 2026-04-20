using FlowerShop.ApplicationData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace FlowerShop.Data
{
    public class DatabaseHelper
    {
        private string _connectionString = "Server=localhost;Database=FlowerShopDB;Trusted_Connection=True;";

        public DatabaseHelper() { }

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ==================== ПОЛЬЗОВАТЕЛИ ====================

        public User FindUser(string login, string password)
        {
            User user = null;
            string query = "SELECT Id, FullName, Login, Password, RoleId, CreatedAt FROM Users WHERE Login = @Login AND Password = @Password";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Login = reader.GetString(2),
                                Password = reader.GetString(3),
                                RoleId = reader.GetInt32(4),
                                CreatedAt = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }
            return user;
        }

        public bool RegisterUser(string fullName, DateTime birthDate, decimal experience, string login, string password, string email, string phone)
        {
            string query = @"INSERT INTO Users (FullName, BirthDate, Experience, Login, Password, PasswordConfirm, Email, Phone, RoleId, CreatedAt) 
                             VALUES (@FullName, @BirthDate, @Experience, @Login, @Password, @Password, @Email, @Phone, @RoleId, @CreatedAt)";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@BirthDate", birthDate);
                    cmd.Parameters.AddWithValue("@Experience", experience);
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@RoleId", 3);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool IsLoginExists(string login)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Login = @Login";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Login", login);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        public bool IsEmailExists(string email)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        // ==================== ТОВАРЫ ====================

        public List<FlowerProduct> GetAllProducts()
        {
            List<FlowerProduct> products = new List<FlowerProduct>();
            string query = @"SELECT Id, Name, Price, CategoryId, IsAvailable, ImageUrl, Rating, DiscountPercent, Stock 
                             FROM FlowerProducts WHERE IsAvailable = 1";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new FlowerProduct
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                CategoryId = reader.GetInt32(3),
                                IsAvailable = reader.GetBoolean(4),
                                ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Rating = reader.IsDBNull(6) ? 4.85m : reader.GetDecimal(6),
                                DiscountPercent = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                Stock = reader.IsDBNull(8) ? 0 : reader.GetInt32(8)
                            });
                        }
                    }
                }
            }
            return products;
        }

        public FlowerProduct GetProductById(int id)
        {
            FlowerProduct product = null;
            string query = "SELECT Id, Name, Price, CategoryId, IsAvailable, ImageUrl, Rating, DiscountPercent, Stock FROM FlowerProducts WHERE Id = @Id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new FlowerProduct
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                CategoryId = reader.GetInt32(3),
                                IsAvailable = reader.GetBoolean(4),
                                ImageUrl = reader.IsDBNull(5) ? null : reader.GetString(5),
                                Rating = reader.IsDBNull(6) ? 4.85m : reader.GetDecimal(6),
                                DiscountPercent = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                Stock = reader.IsDBNull(8) ? 0 : reader.GetInt32(8)
                            };
                        }
                    }
                }
            }
            return product;
        }

        public bool UpdateProductStock(int productId, int newStock)
        {
            string query = "UPDATE FlowerProducts SET Stock = @Stock, IsAvailable = CASE WHEN @Stock > 0 THEN 1 ELSE 0 END WHERE Id = @Id";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Stock", newStock);
                    cmd.Parameters.AddWithValue("@Id", productId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ==================== КАТЕГОРИИ ====================

        public List<CategoryModel> GetAllCategories()
        {
            List<CategoryModel> categories = new List<CategoryModel>();
            string query = "SELECT Id, Name FROM Categories ORDER BY Name";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new CategoryModel
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return categories;
        }

        // ==================== КОРЗИНА ====================

        public int GetOrCreateCart(int userId)
        {
            int cartId = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string checkQuery = "SELECT Id FROM Carts WHERE UserId = @UserId";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@UserId", userId);
                    var result = checkCmd.ExecuteScalar();
                    if (result != null)
                    {
                        cartId = Convert.ToInt32(result);
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO Carts (UserId, UpdatedAt) VALUES (@UserId, GETDATE()); SELECT SCOPE_IDENTITY();";
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@UserId", userId);
                            cartId = Convert.ToInt32(insertCmd.ExecuteScalar());
                        }
                    }
                }
            }
            return cartId;
        }

        public void AddToCart(int userId, int flowerId, int quantity)
        {
            int cartId = GetOrCreateCart(userId);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string checkQuery = "SELECT Id, Quantity FROM CartItems WHERE CartId = @CartId AND FlowerId = @FlowerId";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@CartId", cartId);
                    checkCmd.Parameters.AddWithValue("@FlowerId", flowerId);

                    using (SqlDataReader reader = checkCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int existingId = reader.GetInt32(0);
                            int existingQty = reader.GetInt32(1);
                            reader.Close();

                            string updateQuery = "UPDATE CartItems SET Quantity = @Quantity WHERE Id = @Id";
                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@Quantity", existingQty + quantity);
                                updateCmd.Parameters.AddWithValue("@Id", existingId);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            reader.Close();
                            string insertQuery = "INSERT INTO CartItems (CartId, FlowerId, Quantity) VALUES (@CartId, @FlowerId, @Quantity)";
                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@CartId", cartId);
                                insertCmd.Parameters.AddWithValue("@FlowerId", flowerId);
                                insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        public List<CartItemInfo> GetCartItems(int userId)
        {
            List<CartItemInfo> cartItems = new List<CartItemInfo>();
            int cartId = GetOrCreateCart(userId);

            string query = @"SELECT ci.Id, ci.FlowerId, ci.Quantity, f.Name, f.Price, f.DiscountPercent, f.Stock 
                             FROM CartItems ci
                             INNER JOIN FlowerProducts f ON ci.FlowerId = f.Id
                             WHERE ci.CartId = @CartId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CartId", cartId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cartItems.Add(new CartItemInfo
                            {
                                Id = reader.GetInt32(0),
                                FlowerId = reader.GetInt32(1),
                                Quantity = reader.GetInt32(2),
                                Name = reader.GetString(3),
                                Price = reader.GetDecimal(4),
                                DiscountPercent = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                Stock = reader.IsDBNull(6) ? 0 : reader.GetInt32(6)
                            });
                        }
                    }
                }
            }
            return cartItems;
        }

        public void ClearCart(int userId)
        {
            int cartId = GetOrCreateCart(userId);
            string query = "DELETE FROM CartItems WHERE CartId = @CartId";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CartId", cartId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ==================== ЗАКАЗЫ ====================

        public int CreateOrder(int userId, string deliveryMethod, string deliveryAddress, List<CartItemInfo> cartItems, decimal totalAmount)
        {
            int orderId = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        decimal deliveryPrice = deliveryMethod == "Доставка" ? 500 : 0;

                        string insertOrderQuery = @"INSERT INTO Orders (UserId, TotalAmount, Status, DeliveryMethod, DeliveryAddress, DeliveryPrice, CreatedAt) 
                                                    VALUES (@UserId, @TotalAmount, 'Ожидает оплаты', @DeliveryMethod, @DeliveryAddress, @DeliveryPrice, GETDATE());
                                                    SELECT SCOPE_IDENTITY();";

                        using (SqlCommand orderCmd = new SqlCommand(insertOrderQuery, conn, transaction))
                        {
                            orderCmd.Parameters.AddWithValue("@UserId", userId);
                            orderCmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                            orderCmd.Parameters.AddWithValue("@DeliveryMethod", deliveryMethod);
                            orderCmd.Parameters.AddWithValue("@DeliveryAddress", string.IsNullOrEmpty(deliveryAddress) ? (object)DBNull.Value : deliveryAddress);
                            orderCmd.Parameters.AddWithValue("@DeliveryPrice", deliveryPrice);

                            orderId = Convert.ToInt32(orderCmd.ExecuteScalar());
                        }

                        foreach (var item in cartItems)
                        {
                            decimal finalPrice = item.DiscountPercent > 0 ? item.Price * (100 - item.DiscountPercent) / 100 : item.Price;

                            string insertItemQuery = @"INSERT INTO OrderItems (OrderId, FlowerId, Quantity, UnitPrice) 
                                                       VALUES (@OrderId, @FlowerId, @Quantity, @UnitPrice)";

                            using (SqlCommand itemCmd = new SqlCommand(insertItemQuery, conn, transaction))
                            {
                                itemCmd.Parameters.AddWithValue("@OrderId", orderId);
                                itemCmd.Parameters.AddWithValue("@FlowerId", item.FlowerId);
                                itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                                itemCmd.Parameters.AddWithValue("@UnitPrice", finalPrice);
                                itemCmd.ExecuteNonQuery();
                            }

                            string updateStockQuery = "UPDATE FlowerProducts SET Stock = Stock - @Quantity WHERE Id = @FlowerId";
                            using (SqlCommand stockCmd = new SqlCommand(updateStockQuery, conn, transaction))
                            {
                                stockCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                                stockCmd.Parameters.AddWithValue("@FlowerId", item.FlowerId);
                                stockCmd.ExecuteNonQuery();
                            }
                        }

                        int cartId = GetOrCreateCart(userId);
                        string clearCartQuery = "DELETE FROM CartItems WHERE CartId = @CartId";
                        using (SqlCommand clearCmd = new SqlCommand(clearCartQuery, conn, transaction))
                        {
                            clearCmd.Parameters.AddWithValue("@CartId", cartId);
                            clearCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return orderId;
        }

        public void AddPayment(int orderId, decimal amount, string method)
        {
            string query = @"INSERT INTO Payments (OrderId, Amount, Method, Status, PaidAt) 
                             VALUES (@OrderId, @Amount, @Method, 'Успешно', GETDATE())";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@Method", method);
                    cmd.ExecuteNonQuery();
                }
            }

            string updateOrderQuery = "UPDATE Orders SET Status = 'Оплачен' WHERE Id = @OrderId";
            using (SqlConnection conn2 = new SqlConnection(_connectionString))
            {
                conn2.Open();
                using (SqlCommand cmd2 = new SqlCommand(updateOrderQuery, conn2))
                {
                    cmd2.Parameters.AddWithValue("@OrderId", orderId);
                    cmd2.ExecuteNonQuery();
                }
            }
        }
    }

    public class CartItemInfo
    {
        public int Id { get; set; }
        public int FlowerId { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int DiscountPercent { get; set; }
        public int Stock { get; set; }
        public decimal FinalPrice => DiscountPercent > 0 ? Price * (100 - DiscountPercent) / 100 : Price;
        public decimal TotalPrice => FinalPrice * Quantity;
    }
}