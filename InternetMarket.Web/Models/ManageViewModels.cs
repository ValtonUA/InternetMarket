using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InternetMarket.Web.Models
{
    public class ProductViewModel
    {
        public string ProductName { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public float Rate { get; set; }

        public bool IsVoted { get; set; }

        public float Price { get; set; }
    }

    public class CreateOrEditProductVM
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "ProductName required")]
        [MaxLength(30, ErrorMessage = "Up to 30 symbols")]
        [Remote("ProductAlreadyExistsAsync", 
                "Product", 
                AdditionalFields = "OldProductName", 
                ErrorMessage = "Such product is already exist",
                HttpMethod = "POST")]
        public string ProductName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description required")]
        [MaxLength(100, ErrorMessage = "Up to 100 symbols")]
        public string Description { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category required")]
        [MaxLength(30, ErrorMessage = "Up to 30 symbols")]
        public string Category { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Price required")]
        [Range(0.01, 1000000, ErrorMessage = "Cannot be less than 0.01")]
        public float Price { get; set; }

        public string OldProductName { get; set; }
    }

    public class OrderViewModel
    {
        public int OrderId { get; set; }

        public string Status { get; set; }

        public string Location { get; set; }

        public string UserLogin { get; set; }

        public DateTime Date { get; set; }

        public List<string> ProductNames { get; set; }
    }
}