namespace FlowerShop.ApplicationData
{
    using System;

    public partial class employees
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string role { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public Nullable<System.DateTime> hired_at { get; set; }
        public bool is_active { get; set; }
    }
}
