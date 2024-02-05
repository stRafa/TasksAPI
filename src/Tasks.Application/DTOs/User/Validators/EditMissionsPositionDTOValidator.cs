using FluentValidation;

namespace Tasks.Application.DTOs.User.Validators;

public class EditMissionsPositionDTOValidator : AbstractValidator<EditMissionsPositionDTO>
{
    public EditMissionsPositionDTOValidator()
    {
        RuleFor(dto => dto.MissionId)
            .NotEmpty().WithMessage("MissionId is required.")
            .Must(BeValidGuid).WithMessage("MissionId must be a valid GUID.");

        RuleFor(dto => dto.NewPosition)
            .NotEmpty().WithMessage("NewPosition is required.")
            .GreaterThanOrEqualTo((short)0).WithMessage("NewPosition must be greater than or equal to 0.");
    }

    private bool BeValidGuid(Guid missionId)
    {
        return missionId != Guid.Empty;
    }
}