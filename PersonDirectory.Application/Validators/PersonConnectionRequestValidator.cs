namespace PersonDirectory.Application.Validators;

public class PersonConnectionRequestValidator : AbstractValidator<PersonConnectionRequest>
{
    public PersonConnectionRequestValidator(IStringLocalizer<PersonConnectionRequestValidator> localizer)
    {
        RuleFor(x => x.PersonId)
            .GreaterThan(0).WithMessage(localizer[ErrorMessages.PersonIdRequired]);

        RuleFor(x => x.ConnectedPersonId)
            .GreaterThan(0).WithMessage(localizer[ErrorMessages.ConnectedPersonIdRequired]);

        RuleFor(x => x.ConnectionType)
            .IsInEnum().WithMessage(localizer[ErrorMessages.ConnectionTypeInvalid]);

        RuleFor(x => x)
            .Must(x => x.PersonId != x.ConnectedPersonId)
            .WithMessage(localizer[ErrorMessages.CannotConnectToSelf]);
    }
}