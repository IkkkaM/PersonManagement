namespace PersonDirectory.Application.Validators;

public class PersonCreateRequestValidator : AbstractValidator<PersonCreateRequest>
{
    private readonly IPersonRepository _personRepository;
    private readonly IStringLocalizer<PersonCreateRequestValidator> _localizer;

    public PersonCreateRequestValidator(IPersonRepository personRepository, IStringLocalizer<PersonCreateRequestValidator> localizer)
    {
        _personRepository = personRepository;
        _localizer = localizer;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(_localizer[ErrorMessages.FirstNameRequired])
            .Length(2, 50).WithMessage(_localizer[ErrorMessages.FirstNameLength])
            .Must(BeValidName).WithMessage(_localizer[ErrorMessages.FirstNameInvalidCharacters]);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(_localizer[ErrorMessages.LastNameRequired])
            .Length(2, 50).WithMessage(_localizer[ErrorMessages.LastNameLength])
            .Must(BeValidName).WithMessage(_localizer[ErrorMessages.LastNameInvalidCharacters]);

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage(_localizer[ErrorMessages.GenderInvalid]);

        RuleFor(x => x.PersonalNumber)
            .NotEmpty().WithMessage(_localizer[ErrorMessages.PersonalNumberRequired])
            .Length(11).WithMessage(_localizer[ErrorMessages.PersonalNumberLength])
            .Must(BeAllDigits).WithMessage(_localizer[ErrorMessages.PersonalNumberOnlyDigits]);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage(_localizer[ErrorMessages.DateOfBirthRequired])
            .Must(BeAtLeast18YearsOld).WithMessage(_localizer[ErrorMessages.MinimumAge18Required]);

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage(_localizer[ErrorMessages.CityIdRequired]);

        RuleFor(x => x.PhoneNumbers)
            .NotNull().WithMessage(_localizer[ErrorMessages.PhoneNumbersRequired]);

        RuleForEach(x => x.PhoneNumbers).ChildRules(phoneNumber =>
        {
            phoneNumber.RuleFor(p => p.Type)
                .IsInEnum().WithMessage(_localizer[ErrorMessages.PhoneTypeInvalid]);

            phoneNumber.RuleFor(p => p.Number)
                .NotEmpty().WithMessage(_localizer[ErrorMessages.PhoneNumberRequired])
                .Length(4, 50).WithMessage(_localizer[ErrorMessages.PhoneNumberLength]);
        });

        RuleFor(x => x)
            .Must(HaveConsistentNameLanguage)
            .WithMessage(_localizer[ErrorMessages.NamesLanguageInconsistent]);
    }

    private bool BeValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;

        bool hasGeorgian = name.Any(c => c >= 0x10A0 && c <= 0x10FF);
        bool hasLatin = name.Any(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'));

        return (hasGeorgian && !hasLatin) || (!hasGeorgian && hasLatin);
    }

    private bool HaveConsistentNameLanguage(PersonCreateRequest request)
    {
        bool firstHasGeorgian = request.FirstName.Any(c => c >= 0x10A0 && c <= 0x10FF);
        bool lastHasGeorgian = request.LastName.Any(c => c >= 0x10A0 && c <= 0x10FF);
        return firstHasGeorgian == lastHasGeorgian;
    }

    private bool BeAllDigits(string personalNumber)
    {
        return !string.IsNullOrEmpty(personalNumber) && personalNumber.All(char.IsDigit);
    }

    private bool BeAtLeast18YearsOld(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age))
            age--;
        return age >= 18;
    }
}