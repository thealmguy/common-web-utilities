using CMCS.Common.WebUtilities.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Net;

namespace CMCS.Common.WebUtilities.RedirectRules
{
    public class RedirectToCanonicalHostRule : IRule
    {
        private readonly UrlConfig urlConfig;
        private readonly HttpStatusCode statusCode;

        public RedirectToCanonicalHostRule(UrlConfig urlConfig, HttpStatusCode statusCode = HttpStatusCode.MovedPermanently)
        {
            this.urlConfig = urlConfig;
            this.statusCode = statusCode;
        }

        public virtual void ApplyRule(RewriteContext context)
        {
            var req = context.HttpContext.Request;
            if (req.Host.Host.Equals(urlConfig.CanonicalHost, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = RuleResult.ContinueRules;
                return;
            }

            var host = new HostString(urlConfig.CanonicalHost);
            var newUrl = UriHelper.BuildAbsolute(req.Scheme, host, req.PathBase, req.Path, req.QueryString);
            var response = context.HttpContext.Response;
            response.StatusCode = (int)statusCode;
            response.Headers[HeaderNames.Location] = newUrl;
            context.Result = RuleResult.EndResponse;
        }
    }
}
