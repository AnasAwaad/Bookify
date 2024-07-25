using Bookify.Web.Core.Consts;

namespace Bookify.Web.Core.ViewModels;

public class UpsertAuthorViewModel
{
    public int Id { get; set; }

    [MaxLength(100, ErrorMessage = Errors.MaxLength), Display(Name = "Author")]
    [Remote("IsAuthorAllowed", "Authors", AdditionalFields = "Id", HttpMethod = "Post", ErrorMessage = Errors.Dublicated)]
    public string Name { get; set; } = null!;
}
