using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace CMCS.Common.WebUtilities.RedirectRules
{
    public class RedirectToLowerCaseRule : IRule
    {
        private readonly HttpStatusCode statusCode;
        public RedirectToLowerCaseRule(HttpStatusCode statusCode = HttpStatusCode.MovedPermanently)
        {
            this.statusCode = statusCode;
        }

        public void ApplyRule(RewriteContext context)
        {
            HttpRequest request = context.HttpContext.Request;
            PathString path = context.HttpContext.Request.Path;
            HostString host = context.HttpContext.Request.Host;

            if (path.HasValue && path.Value.Any(char.IsUpper) || host.HasValue && host.Value.Any(char.IsUpper))
            {

                var newUrl = UriHelper.BuildAbsolute(request.Scheme.ToLower(), new HostString(host.ToString().ToLower()), request.PathBase.ToString().ToLower(), request.Path.ToString().ToLower(), request.QueryString);
                var response = context.HttpContext.Response;
                response.StatusCode = (int)statusCode;
                response.Headers[HeaderNames.Location] = newUrl;
                context.Result = RuleResult.EndResponse;
            }
            else
            {
                context.Result = RuleResult.ContinueRules;
            }
        }
    }
}
