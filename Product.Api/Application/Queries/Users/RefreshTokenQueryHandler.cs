using System.Threading;
using System.Threading.Tasks;
using EventBus.Abstractions;
using Product.Api.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Product.Api.Application.Queries.Users;

// public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, AuthViewModel.GetTokenOutput>
// {
//     private readonly IUserStore<> userStore;
//     private readonly IEventBus eventHandler;
//
//     public RefreshTokenQueryHandler(
//         IUserStore userStore, IEventBus eventHandler)
//     {
//         this.userStore = userStore;
//         this.eventHandler = eventHandler;
//     }
//
//     public async Task<AuthViewModel.GetTokenOutput> Handle(RefreshTokenQuery request,
//         CancellationToken cancellationToken)
//     {
//         var user = await userStore.GetTokenByRefreshAsync(request.RefreshToken);
//
//         // return await eventHandler.SendMediator(new LoginCommand(user.UserName, user.Password));
//         return null;
//     }
// }