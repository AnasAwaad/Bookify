

namespace Bookify.Web.Core.ViewModels
{
	public class Category
	{
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }=DateTime.Now;
        public DateTime? LastUpdatedOn { get; set; }
    }
}
