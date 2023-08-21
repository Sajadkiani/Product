using AutoMapper;
using IdentityGrpcClient;
using Product.Infrastructure.Models;

namespace Product.Infrastructure.MapperProfiles
{
    public class AutoMapperInfraProfile : Profile
    {
        public AutoMapperInfraProfile()
        {
            CreateMap<RefreshTokenResponse, AuthModel.GetTokenModel>();
        }
    }
}