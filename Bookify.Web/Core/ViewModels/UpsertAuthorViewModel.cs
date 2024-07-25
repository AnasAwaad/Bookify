using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Core.ViewModels;

public class UpsertAuthorViewModel
{
    public int Id { get; set; }
    [MaxLength(100, ErrorMessage = "max length is 100 chars.")]
    public string Name { get; set; } = null!;
}
