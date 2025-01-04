using FluentValidation;
using MagicVilla_CouponAPI.Models.DTO;

namespace MagicVilla_CouponAPI.Validations
{
    public class CouponCreateValidation : AbstractValidator<CouponCreateDTO>
    {
        public CouponCreateValidation()
        {
            RuleFor(model => model.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(model => model.Percent).InclusiveBetween(1, 100).WithMessage("Percent must be between 1 to 100");
            RuleFor(model => model.IsActive).NotEmpty().WithMessage("IsActive is required");
        }
    }
}
