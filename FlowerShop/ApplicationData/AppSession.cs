namespace FlowerShop.ApplicationData
{
    public static class AppSession
    {
        public static User CurrentUser { get; set; }
        public static string CurrentRole { get; set; } = "user";
        public static int? CurrentUserId => CurrentUser?.Id;
        public static bool IsAuthenticated => CurrentUser != null;
        public static bool IsAdmin => CurrentRole == "admin";
    }
}