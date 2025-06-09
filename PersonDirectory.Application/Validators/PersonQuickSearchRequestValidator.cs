namespace PersonDirectory.Application.Validators;

public class PersonQuickSearchRequestValidator : AbstractValidator<PersonQuickSearchRequest>
{
    public PersonQuickSearchRequestValidator(IStringLocalizer<PersonQuickSearchRequestValidator> localizer)
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty().WithMessage(localizer[ErrorMessages.SearchTermRequired]);

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage(localizer[ErrorMessages.PageNumberMustBePositive]);

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage(localizer[ErrorMessages.PageSizeMustBePositive])
            .LessThanOrEqualTo(100).WithMessage(localizer[ErrorMessages.PageSizeMaximum]);
    }
}

public class PersonDetailedSearchRequestValidator : AbstractValidator<PersonDetailedSearchRequest>
{
    public PersonDetailedSearchRequestValidator(IStringLocalizer<PersonDetailedSearchRequestValidator> localizer)
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage(localizer[ErrorMessages.PageNumberMustBePositive]);

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage(localizer[ErrorMessages.PageSizeMustBePositive])
            .LessThanOrEqualTo(100).WithMessage(localizer[ErrorMessages.PageSizeMaximum]);

        When(x => !string.IsNullOrEmpty(x.PersonalNumber), () =>
        {
            RuleFor(x => x.PersonalNumber)
                .Length(11).WithMessage(localizer[ErrorMessages.PersonalNumberLength])
                .Must(BeAllDigits).WithMessage(localizer[ErrorMessages.PersonalNumberOnlyDigits]);
        });

        When(x => x.CityId.HasValue, () =>
        {
            RuleFor(x => x.CityId)
                .GreaterThan(0).WithMessage(localizer[ErrorMessages.CityIdRequired]);
        });
    }

    private bool BeAllDigits(string? personalNumber)
    {
        return string.IsNullOrEmpty(personalNumber) || personalNumber.All(char.IsDigit);
    }
}