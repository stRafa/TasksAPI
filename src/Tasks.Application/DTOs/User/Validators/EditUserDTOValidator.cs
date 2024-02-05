using FluentValidation;

namespace Tasks.Application.DTOs.User.Validators;

public class EditUserDTOValidator : AbstractValidator<EditUserDTO>
{
    public EditUserDTOValidator()
    {
        RuleFor(user => user.Id)
            .NotEmpty().WithMessage("Id is required");
        
        When(user => !string.IsNullOrEmpty(user.Name), () =>
        {
            RuleFor(user => user.Name)
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        });
        
        When(user => !string.IsNullOrEmpty(user.Email), () =>
        {
            RuleFor(user => user.Email)
                .EmailAddress().WithMessage("Invalid email address");
        });
    }
}