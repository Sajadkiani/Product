using System.Data;
using Dapper;
using Identity.Infrastructure.Dapper;
using Product.Domain.IServices;

namespace Product.Infrastructure.ORM.Dapper;

//user this class in cqrs queries 
public class DapperQueryExecutor : IQueryExecutor
{
    private readonly IDbConnection context;
    public DapperQueryExecutor(
        DapperContext context
        )
    {
        this.context = context.CreateConnection();
    }

    public async Task<IEnumerable<TResponse>> QueryAsync<TResponse>(string rawQuery) where TResponse : class
    {
        return await context.QueryAsync<TResponse>(rawQuery);
    }
}