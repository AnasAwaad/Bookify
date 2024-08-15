using Bookify.Web.Core.Consts;
using UserManagement.Consts;

namespace Bookify.Web.Core.ViewModels;

public class UpsertAuthorViewModel
{
    public int Id { get; set; }

    [MaxLength(100, ErrorMessage = Errors.MaxLength), Display(Name = "Author")]
    [Remote("IsAuthorAllowed", "Authors", AdditionalFields = "Id", HttpMethod = "Post", ErrorMessage = Errors.Dublicated)]
    [RegularExpression(RegexPattern.CharactersOnly_Eng,ErrorMessage =Errors.OnlyEnglishLetters)]
    public string Name { get; set; } = null!;
}
