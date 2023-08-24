using ECommerceAPI.Application.ViewModels.Products;
using FluentValidation;

namespace ECommerceAPI.Application.Validators.Products
{
    public class CreateProductValidator : AbstractValidator<VM_Create_Product>
    {
        public CreateProductValidator()
        {
            RuleFor(p => p.Name).NotEmpty().NotNull().WithMessage("You can not pass null for Product Name").MaximumLength(100).MinimumLength(2).WithMessage("You can use min 2, max 100 characters for Product Name");
            RuleFor(p => p.Stock).NotEmpty().NotNull().WithMessage("You can not pass null for Product Stock").Must(s => s >= 0).WithMessage("You need to give 0 or higher value for Product Stock");
            RuleFor(p => p.Price).NotEmpty().NotNull().WithMessage("You can not pass null for Product Price").Must(s => s >= 0).WithMessage("You need to give 0 or higher value for Product Price");
        }
    }
}
