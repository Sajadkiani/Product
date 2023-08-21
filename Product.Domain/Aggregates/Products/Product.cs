using Product.Domain.SeedWork;
using Product.Domain.Validations.Users;

namespace Product.Domain.Aggregates.Products;

public class Product : Entity, IAggregateRoot
    {
        private Product()
        {
            
        }
        public Product(string name, IProductBcScopeValidation bcScopeValidation)
        {
            Name = name;
            
            //TODO: all invariants and data consistencies must put here 
            // Validate(bcScopeValidation);
            // AddDomainEvent(new TestDomainEvent("UserName"));
        }

        public string Name { get; private set; }
        
        private void Validate(IProductBcScopeValidation bcScopeValidation)
        {
          
        }
    }