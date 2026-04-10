using FluentValidation;
using BackendService.Core.DTOs.Product.Requests;

public class AddProductRequestDtoValidator : AbstractValidator<AddProductRequestDto>
{
    public AddProductRequestDtoValidator()
    {
        // ProductName
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
            .MaximumLength(255).WithMessage("Tên sản phẩm tối đa 255 ký tự");
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId không hợp lệ");

        RuleFor(x => x.SupplierId)
            .NotEmpty().WithMessage("SupplierId không hợp lệ");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Giá bán phải lớn hơn 0");
        RuleFor(x => x.Cost)
            .GreaterThanOrEqualTo(0).WithMessage("Giá nhập không được âm");
        RuleFor(x => x.Image_Url)
            .NotEmpty().WithMessage("Ảnh sản phẩm không được để trống")
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("URL ảnh không hợp lệ");
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Mô tả tối đa 1000 ký tự");
    }
}