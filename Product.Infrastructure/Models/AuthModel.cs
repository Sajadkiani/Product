namespace Product.Infrastructure.Models;

public class AuthModel
{
    public class GetTokenModel
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}