using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Persistence.Contexts;

namespace ECommerceAPI.Persistence.Repositories
{
    public class ProductImageFileWriteRepository : WriteRepository<ECommerceAPI.Domain.Entities.ProductImageFile>, IProductImageFileWriteRepository
    {
        public ProductImageFileWriteRepository(ECommerceAPIDbContext context) : base(context)
        {
        }
    }
}
