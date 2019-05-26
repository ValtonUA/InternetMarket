using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternetMarket.Domain.Core
{
    public class Role
    {
        [Key]
        public string RoleName { get; set; }
    }
}
