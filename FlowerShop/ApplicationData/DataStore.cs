using System;
using System.Collections.Generic;

namespace FlowerShop.ApplicationData
{
    public static class DataStore
    {
        private static ShopModel _shop;

        public static ShopModel Shop
        {
            get
            {
                if (_shop == null)
                {
                    _shop = new ShopModel();
                }
                return _shop;
            }
        }

        public static event EventHandler ProductsUpdated;

        public static void RefreshProducts()
        {
            ProductsUpdated?.Invoke(null, null);
        }
    }
}