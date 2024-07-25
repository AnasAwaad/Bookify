using System.Globalization;

namespace Bookify.Web.Core.Models;

public class Author
{
	public Author()
	{
		CreatedOn = DateTime.Now;
		CreatedOn.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
		IsActive = true;
	}
	public int Id { get; set; }
	[MaxLength(100)]
	public string Name { get; set; } = null!;
	public bool IsActive { get; set; }
	public DateTime CreatedOn { get; set; }
	public DateTime? LastUpdatedOn { get; set; }
}
