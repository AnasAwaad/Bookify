using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Core.ViewModels
{
    public class UpsertCategoryViewModel
    {
        public int Id { get; set; }
        [MaxLength(100,ErrorMessage ="max length is 100 chars.")]
        [Remote("IsCategoryAllowed","Categories",AdditionalFields ="Id",HttpMethod ="Post",ErrorMessage ="Category Name is already exists")]
        public string Name { get; set; } = null!;
    }
}
