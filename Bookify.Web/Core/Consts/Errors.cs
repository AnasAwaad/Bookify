namespace Bookify.Web.Core.Consts;

public static class Errors
{
    public const string MaxLength = "Length cannot be more than {1} characters";
    public const string Dublicated = "{0} with the same name is already exists!";
    public const string BookAuthorDublicated = "{0} with the same author name is already exists!";
    public const string BookTitleDublicated = "{0} with the same book title is already exists!";
    public const string AllowedExtensions = "Only .jpg , .png and .jpeg are allowed";
    public const string AllowedSize = "Image cannot be more than 1 megabyte!";
    public const string NotAllowedDate = "The Date can't be in the future!";
    public const string EditionNumberRange = "{0} must be in range {1} to {2}!";
    public const string MinMaxLength = "The {0} must be at least {2} and at max {1} characters long.";
    public const string ConfirmPasswordNotMatch = "The password and confirmation password do not match.";
    public const string WeakPassword = "Your password must be at least 6 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character (e.g., @, $, !). Please update your password to meet these requirements.";
    public const string AllowUsername = "Username must start with a letter and be 3 to 20 characters long. Only letters, numbers, underscores, hyphens, @, and # are allowed.";
    public const string NotAllowedNationalId = "Invalid National ID, Ensure the ID starts with 2 or 3 and is exactly 14 digits long.";
    public const string PasswordConfirmed = "The password and confirmation password do not match.";
    public const string OnlyEnglishLetters = "Only English letters are allowed.";
    public const string OnlyArabicLetters = "Only Arabic letters are allowed.";
    public const string OnlyNumbersAndLetters = "Only Arabic/English letters or digits are allowed.";
    public const string DenySpecialCharacters = "Special characters are not allowed.";
    public const string RequiredField = "{0} is requied";
    public const string InvalidPhoneNumber = "Invalid phone number";
    public const string MaxAllowedCopies = "this subscriber reach to max allowed copies for rental";
    public const string BlackListedSubscriber = "This subscriber is black listed";
    public const string InActiveSubscriber = "This subscriber is inactive";
    public const string CopyInRental = "This copy is already in rental with another person.";
    public const string NotAvailableForRental = "This book/copy is not available for rental";
    public const string InvalidSerialNumber = "Invalid serial number";
}
