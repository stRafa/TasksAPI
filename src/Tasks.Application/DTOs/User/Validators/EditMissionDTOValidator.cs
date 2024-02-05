using FluentValidation;

namespace Tasks.Application.DTOs.User.Validators;

public class EditMissionDTOValidator : AbstractValidator<EditMissionDTO>
{
    public EditMissionDTOValidator()
    {
        RuleFor(dto => dto.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(5).WithMessage("Title must have at least 5 characters.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(dto => dto.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(5).WithMessage("Description must have at least 5 characters.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(dto => dto.Id)
            .NotEmpty().WithMessage("Id is required.")
            .Must(BeValidGuid).WithMessage("Id must be a valid GUID.");

        RuleFor(dto => dto.Status)
            .NotEmpty().WithMessage("Status is Required");
    }
    
    private bool BeValidGuid(Guid userId)
    {
        return userId != Guid.Empty;
    }
}