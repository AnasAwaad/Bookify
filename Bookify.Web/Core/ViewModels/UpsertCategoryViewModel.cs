namespace Bookify.Web.Core.ViewModels
{
    public class UpsertCategoryViewModel
    {
        public int Id { get; set; }
        [MaxLength(100,ErrorMessage ="max length is 100 chars.")]
        public string Name { get; set; } = null!;
    }
}
