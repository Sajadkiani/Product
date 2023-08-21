using Product.Infrastructure.Models;

namespace Product.Infrastructure.Clients.Grpc;

public interface IIdentityClient
{
    Task<AuthModel.GetTokenModel> RefreshTokenAsync(string refreshToken);
}