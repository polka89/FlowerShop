using System;
using System.Data.SqlClient;
using System.Linq;

namespace FlowerShop.ApplicationData
{
    public static class AuthRepository
    {
        public static bool LoginExists(string login)
        {
            return AppConnect.model01.Database.SqlQuery<int>(
                "SELECT COUNT(1) FROM dbo.users_auth WHERE login = @login",
                new SqlParameter("@login", login)).FirstOrDefault() > 0;
        }

        public static bool TryAuthorize(string login, string password, out customers customer, out string role)
        {
            customer = null;
            role = "user";

            var row = AppConnect.model01.Database.SqlQuery<AuthRow>(@"
SELECT TOP 1 ua.customer_id AS CustomerId, r.name AS RoleName
FROM dbo.users_auth ua
INNER JOIN dbo.roles r ON r.id = ua.role_id
WHERE ua.login = @login AND ua.[password] = @password",
                new SqlParameter("@login", login),
                new SqlParameter("@password", password)).FirstOrDefault();

            if (row == null)
            {
                return false;
            }

            customer = AppConnect.model01.customers.FirstOrDefault(c => c.id == row.CustomerId);
            role = string.IsNullOrWhiteSpace(row.RoleName) ? "user" : row.RoleName;
            return customer != null;
        }

        public static void Register(customers customer, string login, string password, string role = "user")
        {
            AppConnect.model01.customers.Add(customer);
            AppConnect.model01.SaveChanges();

            int roleId = AppConnect.model01.Database.SqlQuery<int>(
                "SELECT TOP 1 id FROM dbo.roles WHERE name = @name",
                new SqlParameter("@name", role)).FirstOrDefault();

            if (roleId == 0)
            {
                AppConnect.model01.Database.ExecuteSqlCommand(
                    "INSERT INTO dbo.roles(name) VALUES(@name)",
                    new SqlParameter("@name", role));

                roleId = AppConnect.model01.Database.SqlQuery<int>(
                    "SELECT TOP 1 id FROM dbo.roles WHERE name = @name",
                    new SqlParameter("@name", role)).FirstOrDefault();
            }

            AppConnect.model01.Database.ExecuteSqlCommand(@"
INSERT INTO dbo.users_auth(customer_id, login, [password], role_id)
VALUES(@customer_id, @login, @password, @role_id)",
                new SqlParameter("@customer_id", customer.id),
                new SqlParameter("@login", login),
                new SqlParameter("@password", password),
                new SqlParameter("@role_id", roleId));
        }

        private class AuthRow
        {
            public int CustomerId { get; set; }
            public string RoleName { get; set; }
        }
    }
}
