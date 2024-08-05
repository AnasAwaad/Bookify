using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Bookify.Web.Helper;

// 1- html tag that we will apply tag helper on it (<a></a>)
// 2- name of attribute of tag heper   ( <a active-when="Home"></a> )
// 3- value of active-when  , must be same of active-when but in pascal case           ( "Home") =>Controller name

[HtmlTargetElement("a", Attributes = "active-when")]
public class ActiveTag : TagHelper
{
    public string? ActiveWhen { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext? ViewContextData { get; set; }


    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // output ==> is the element that have attribute
        if (string.IsNullOrEmpty(ActiveWhen))
            return;

        var currentController = ViewContextData?.RouteData.Values["controller"]?.ToString() ?? string.Empty;
        if (currentController!.Equals(ActiveWhen))
        {
            if (output.Attributes.ContainsName("class"))
                output.Attributes.SetAttribute("class", $"{output.Attributes["class"].Value} active");
            else
                output.Attributes.SetAttribute("class", "active");
        }
    }
}
