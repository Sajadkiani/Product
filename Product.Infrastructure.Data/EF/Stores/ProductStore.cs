using Product.Domain.Aggregates.Products;

namespace Product.Infrastructure.Data.EF.Stores;

public class ProductStore : Repository<Domain.Aggregates.Products.Product, int>, IProductStore
{
    public ProductStore(AppDbContext context) : base(context)
    {
    }
}