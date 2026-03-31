namespace FlowerShop.ApplicationData
{
    public static class AppSession
    {
        public static customers CurrentUser { get; set; }
        public static string CurrentRole { get; set; } = "user";
    }
}
