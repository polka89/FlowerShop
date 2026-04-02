namespace FlowerShop.ApplicationData
{
    using System;

    public partial class inventory_batches
    {
        public int id { get; set; }
        public int flower_id { get; set; }
        public int delivery_id { get; set; }
        public int quantity { get; set; }
        public decimal purchase_price { get; set; }
        public Nullable<System.DateTime> expiration_date { get; set; }
    }
}
