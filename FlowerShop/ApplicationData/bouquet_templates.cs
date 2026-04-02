namespace FlowerShop.ApplicationData
{
    public partial class bouquet_templates
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public int category_id { get; set; }
        public bool is_available { get; set; }
    }
}
