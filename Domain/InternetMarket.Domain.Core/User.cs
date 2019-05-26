namespace InternetMarket.Domain.Core
{
    public class User
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string Login { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
    }
}
