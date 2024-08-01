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
}
