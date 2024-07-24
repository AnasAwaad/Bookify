using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System;

namespace Bookify.Web.Core.Models
{
    [Index("Name",IsUnique =true)]
    public class Category
	{
		public Category()
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
}
