using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternetMarket.Domain.Core;

namespace InternetMarket.Infrastructure.Data
{
    public class OrderContext : DbContext
    {
        public DbSet<Status> AllStatus { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }

        public OrderContext()
            : base("InternetMarket")
        { }
    }
}
