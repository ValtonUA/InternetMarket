using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using InternetMarket.Domain.Core;

namespace InternetMarket.Infrastructure.Data
{
    public class UserContext : DbContext
    {
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserProduct> UserProducts { get; set; } // Carts

        public UserContext()
            : base("InternetMarket")
        { }
    }
}
