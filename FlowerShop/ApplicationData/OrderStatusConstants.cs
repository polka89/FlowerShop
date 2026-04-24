using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShop.ApplicationData
{
    public static class OrderStatusConstants
    {
        public const string Pending = "Ожидает обработки";
        public const string Confirmed = "Подтвержден";
        public const string InDelivery = "В доставке";
        public const string Completed = "Выполнен";
        public const string Cancelled = "Отменен";

        public static string[] GetAllStatuses()
        {
            return new[] { Pending, Confirmed, InDelivery, Completed, Cancelled };
        }
    }
}