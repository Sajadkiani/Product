using AutoMapper;
using IdentityGrpcClient;
using Product.Infrastructure.Models;

namespace Product.Infrastructure.Clients.Grpc;

public class IdentityGrpcClientImpl : IIdentityClient
{
    private readonly IdentityGrpc.IdentityGrpcClient client;
    private readonly IMapper mapper;

    public IdentityGrpcClientImpl(IdentityGrpc.IdentityGrpcClient client, IMapper mapper)
    {
        this.client = client;
        this.mapper = mapper;
    }

    public async Task<AuthModel.GetTokenModel> RefreshTokenAsync(string refreshToken)
    {
        var token = await client.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = refreshToken });
        return mapper.Map<AuthModel.GetTokenModel>(token);
    }
}