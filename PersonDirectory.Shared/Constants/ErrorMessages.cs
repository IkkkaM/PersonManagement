namespace PersonDirectory.Shared.Constants;

public static class ErrorMessages
{
    public const string FirstNameRequired = "FirstNameRequired";
    public const string FirstNameLength = "FirstNameLength";
    public const string FirstNameInvalidCharacters = "FirstNameInvalidCharacters";

    public const string LastNameRequired = "LastNameRequired";
    public const string LastNameLength = "LastNameLength";
    public const string LastNameInvalidCharacters = "LastNameInvalidCharacters";

    public const string NamesLanguageInconsistent = "NamesLanguageInconsistent";

    public const string GenderRequired = "GenderRequired";
    public const string GenderInvalid = "GenderInvalid";

    public const string PersonalNumberRequired = "PersonalNumberRequired";
    public const string PersonalNumberLength = "PersonalNumberLength";
    public const string PersonalNumberOnlyDigits = "PersonalNumberOnlyDigits";
    public const string PersonalNumberAlreadyExists = "PersonalNumberAlreadyExists";

    public const string DateOfBirthRequired = "DateOfBirthRequired";
    public const string MinimumAge18Required = "MinimumAge18Required";

    public const string CityIdRequired = "CityIdRequired";
    public const string CityNotFound = "CityNotFound";

    public const string PhoneNumbersRequired = "PhoneNumbersRequired";
    public const string PhoneTypeRequired = "PhoneTypeRequired";
    public const string PhoneTypeInvalid = "PhoneTypeInvalid";
    public const string PhoneNumberRequired = "PhoneNumberRequired";
    public const string PhoneNumberLength = "PhoneNumberLength";

    public const string PersonIdRequired = "PersonIdRequired";
    public const string ConnectedPersonIdRequired = "ConnectedPersonIdRequired";
    public const string ConnectionTypeRequired = "ConnectionTypeRequired";
    public const string ConnectionTypeInvalid = "ConnectionTypeInvalid";
    public const string CannotConnectToSelf = "CannotConnectToSelf";
    public const string ConnectionAlreadyExists = "ConnectionAlreadyExists";

    public const string SearchTermRequired = "SearchTermRequired";
    public const string PageNumberMustBePositive = "PageNumberMustBePositive";
    public const string PageSizeMustBePositive = "PageSizeMustBePositive";
    public const string PageSizeMaximum = "PageSizeMaximum";

    public const string PersonNotFound = "PersonNotFound";
    public const string ValidationFailed = "ValidationFailed";
    public const string InternalServerError = "InternalServerError";
    public const string UnauthorizedAccess = "UnauthorizedAccess";
    public const string ForbiddenAccess = "ForbiddenAccess";

    public const string FileUploadFailed = "FileUploadFailed";
    public const string InvalidFileFormat = "InvalidFileFormat";
    public const string FileTooLarge = "FileTooLarge";
    public const string FileNotFound = "FileNotFound";

    public const string DatabaseConnectionFailed = "DatabaseConnectionFailed";
    public const string DatabaseOperationFailed = "DatabaseOperationFailed";
    public const string TransactionFailed = "TransactionFailed";
}
