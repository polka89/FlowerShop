namespace FlowerShop.ApplicationData
{
    using System;

    public partial class suppliers
    {
        public int id { get; set; }
        public string company_name { get; set; }
        public string contact_person { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
    }
}
