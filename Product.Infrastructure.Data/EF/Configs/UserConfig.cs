using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Product.Infrastructure.Data.EF.Configs;

public class UserConfig : IEntityTypeConfiguration<Domain.Aggregates.Products.Product>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Products.Product> builder)
    {
        builder.ToTable("Products").HasKey(item => item.Id);
        
        // builder
        //     .HasMany(item => item.Tokens)
        //     .WithOne(item => item.User)
        //     .HasForeignKey(item => item.UserId).OnDelete(DeleteBehavior.Cascade);

        //EF access the OrderItem collection property through its backing field
        // var nav = builder.Metadata.FindNavigation(nameof(User.Tokens));
        // nav!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}