using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InternetMarket.Domain.Core;

namespace InternetMarket.Infrastructure.Data
{
    public class ProductContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVote> ProductVotes { get; set; }

        public ProductContext()
            : base("InternetMarket")
        { }
    }
}
