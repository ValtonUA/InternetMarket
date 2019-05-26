namespace InternetMarket.Domain.Core
{
    public class Product
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string ProductName { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public float Rate { get; set; }

        public int NumberOfVotes { get; set; }

        public float Price { get; set; }
    }
}
