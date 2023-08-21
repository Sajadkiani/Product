using MediatR;

namespace Product.Api.Application.Commands.Users;

public class TestCommand : IRequest<bool>
{
    public string UserName { get; init; }
}
