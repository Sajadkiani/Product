using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Product.Infrastructure.Exceptions;

namespace Product.Infrastructure.Clients.Grpc;

public class ExceptionInterceptor: Interceptor
{
    private readonly ILogger<ExceptionInterceptor> logger;
 
    public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
    {
        this.logger = logger;
    }
     
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (MyApplicationException.Unauthorized exception)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, AppMessages.Unauthenticated.message));
        }
        catch (Exception exception)
        {
            throw new RpcException(new Status(StatusCode.Internal, AppMessages.InternalError.message));
        }
    }
}