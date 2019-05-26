using System;

namespace InternetMarket.Domain.Core
{
    public class Order
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int OrderId { get; set; }

        public string Status { get; set; }

        public string Location { get; set; }

        public string UserLogin { get; set; }

        public DateTime Date { get; set; }
    }
}
