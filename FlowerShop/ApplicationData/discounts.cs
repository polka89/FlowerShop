namespace FlowerShop.ApplicationData
{
    using System;

    public partial class discounts
    {
        public int id { get; set; }
        public string name { get; set; }
        public decimal discount_percent { get; set; }
        public Nullable<System.DateTime> start_date { get; set; }
        public Nullable<System.DateTime> end_date { get; set; }
        public bool is_active { get; set; }
    }
}
