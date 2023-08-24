using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Persistence.Contexts;

namespace ECommerceAPI.Persistence.Repositories
{
    public class InvoiceFileWriteRepository : WriteRepository<ECommerceAPI.Domain.Entities.InvoiceFile>, IInvoiceFileWriteRepository
    {
        public InvoiceFileWriteRepository(ECommerceAPIDbContext context) : base(context)
        {
        }
    }
}
