using UserManagement.Consts;

namespace Bookify.Web.Core.ViewModels
{
    public class UpsertCategoryViewModel
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = Errors.MaxLength), Display(Name = "Category")]
        [Remote("IsCategoryAllowed", "Categories", AdditionalFields = "Id",ErrorMessage = Errors.Dublicated)]
        [RegularExpression(RegexPattern.CharactersOnly_Eng, ErrorMessage = Errors.OnlyEnglishLetters)]

        public string Name { get; set; } = null!;
    }
}
