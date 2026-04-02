using System;
using System.Data.Entity;

namespace FlowerShop.ApplicationData
{
    public static class DbBootstrapper
    {
        public static void EnsureExtendedSchema()
        {
            // В существующей БД уже есть 9 таблиц из EDMX.
            // Ниже создаем еще 6 служебных таблиц, чтобы итогово было 15.
            ExecuteIfMissing("roles", @"
CREATE TABLE dbo.roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL UNIQUE,
    created_at DATETIME NULL DEFAULT(GETDATE())
);");

            ExecuteIfMissing("users_auth", @"
CREATE TABLE dbo.users_auth (
    id INT IDENTITY(1,1) PRIMARY KEY,
    customer_id INT NOT NULL,
    login NVARCHAR(100) NOT NULL UNIQUE,
    [password] NVARCHAR(256) NOT NULL,
    role_id INT NOT NULL,
    created_at DATETIME NULL DEFAULT(GETDATE()),
    CONSTRAINT FK_users_auth_customers FOREIGN KEY(customer_id) REFERENCES dbo.customers(id),
    CONSTRAINT FK_users_auth_roles FOREIGN KEY(role_id) REFERENCES dbo.roles(id)
);");

            ExecuteIfMissing("favorites", @"
CREATE TABLE dbo.favorites (
    id INT IDENTITY(1,1) PRIMARY KEY,
    customer_id INT NOT NULL,
    flower_id INT NOT NULL,
    created_at DATETIME NULL DEFAULT(GETDATE()),
    CONSTRAINT FK_favorites_customers FOREIGN KEY(customer_id) REFERENCES dbo.customers(id),
    CONSTRAINT FK_favorites_flowers FOREIGN KEY(flower_id) REFERENCES dbo.flowers(id)
);");

            ExecuteIfMissing("flower_images", @"
CREATE TABLE dbo.flower_images (
    id INT IDENTITY(1,1) PRIMARY KEY,
    flower_id INT NOT NULL,
    file_name NVARCHAR(260) NOT NULL,
    sort_order INT NOT NULL DEFAULT(0),
    created_at DATETIME NULL DEFAULT(GETDATE()),
    CONSTRAINT FK_flower_images_flowers FOREIGN KEY(flower_id) REFERENCES dbo.flowers(id)
);");

            ExecuteIfMissing("carts", @"
CREATE TABLE dbo.carts (
    id INT IDENTITY(1,1) PRIMARY KEY,
    customer_id INT NOT NULL,
    created_at DATETIME NULL DEFAULT(GETDATE()),
    is_active BIT NOT NULL DEFAULT(1),
    CONSTRAINT FK_carts_customers FOREIGN KEY(customer_id) REFERENCES dbo.customers(id)
);");

            ExecuteIfMissing("cart_items", @"
CREATE TABLE dbo.cart_items (
    id INT IDENTITY(1,1) PRIMARY KEY,
    cart_id INT NOT NULL,
    flower_id INT NOT NULL,
    quantity INT NOT NULL DEFAULT(1),
    created_at DATETIME NULL DEFAULT(GETDATE()),
    CONSTRAINT FK_cart_items_carts FOREIGN KEY(cart_id) REFERENCES dbo.carts(id),
    CONSTRAINT FK_cart_items_flowers FOREIGN KEY(flower_id) REFERENCES dbo.flowers(id)
);");

            SeedRoles();
        }

        private static void ExecuteIfMissing(string tableName, string createSql)
        {
            string sql = $@"
IF OBJECT_ID(N'dbo.{tableName}', N'U') IS NULL
BEGIN
    {createSql}
END";

            AppConnect.model01.Database.ExecuteSqlCommand(sql);
        }

        private static void SeedRoles()
        {
            AppConnect.model01.Database.ExecuteSqlCommand(@"
IF NOT EXISTS (SELECT 1 FROM dbo.roles WHERE name = N'admin')
    INSERT INTO dbo.roles(name) VALUES (N'admin');
IF NOT EXISTS (SELECT 1 FROM dbo.roles WHERE name = N'user')
    INSERT INTO dbo.roles(name) VALUES (N'user');
");
        }
    }
}
