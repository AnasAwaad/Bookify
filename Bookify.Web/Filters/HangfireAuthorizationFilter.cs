﻿using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;

namespace Bookify.Web.Filters;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private string _policyName;

    public HangfireAuthorizationFilter(string policyName)
    {
        _policyName = policyName;
    }

    public bool Authorize([NotNull] DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var authService = httpContext.RequestServices.GetRequiredService<IAuthorizationService>();
        var isAuthorize = authService.AuthorizeAsync(httpContext.User, _policyName)
                                    .ConfigureAwait(false)
                                    .GetAwaiter()
                                    .GetResult()
                                    .Succeeded;
        return isAuthorize;
    }

}
