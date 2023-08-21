using Product.Domain.SeedWork;

namespace Product.Domain.Aggregates.Products;

public interface IProductStore : IRepository<Product, int>
{
    
}