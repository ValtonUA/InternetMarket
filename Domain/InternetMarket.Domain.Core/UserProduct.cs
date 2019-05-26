using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetMarket.Domain.Core
{
    public class UserProduct // Literally It`s a cart
    {
        public int Id { get; set; }

        public string UserLogin { get; set; }

        public string ProductName { get; set; }
    }
}
