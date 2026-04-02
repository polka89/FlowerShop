namespace FlowerShop.ApplicationData
{
    using System;

    public partial class deliveries
    {
        public int id { get; set; }
        public int supplier_id { get; set; }
        public Nullable<System.DateTime> delivery_date { get; set; }
        public decimal total_cost { get; set; }
        public string comment { get; set; }
    }
}
