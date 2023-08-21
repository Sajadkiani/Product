using Product.Api.ViewModels;
using MediatR;
using Product.Infrastructure.Models;

namespace Product.Api.Application.Queries.Users;

public class RefreshTokenQuery : IRequest<AuthModel.GetTokenModel>
{
    public string RefreshToken { get; set; }
}