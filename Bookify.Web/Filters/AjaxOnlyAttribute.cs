using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Bookify.Web.Filters
{
    // Custom attribute to restrict action methods to AJAX requests only
    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        // Override the IsValidForRequest method to implement the AJAX request check
        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            // Retrieve the current HTTP request from the routing context
            var request = routeContext.HttpContext.Request;

            // Check if the request contains the 'X-Requested-With' header with the value 'XMLHttpRequest'
            // This header is typically set by JavaScript libraries (like jQuery) for AJAX requests
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
