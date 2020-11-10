using CMCS.Common.WebUtilities.Objects;
using CMCS.Common.WebUtilities.RedirectRules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using NSubstitute;
using NUnit.Framework;
using System.Net;

namespace CMCS.Common.WebUtilities.Testing.RedirectRules
{
    public class RedirectToCanonicalHostRuleTests
    {
        private RedirectToCanonicalHostRule rule;
        private UrlConfig urlConfig;

        [SetUp]
        public void Setup()
        {
            urlConfig = new UrlConfig() { CanonicalHost = "www.foo.bar", HttpPort = 80, HttpsPort = 443 };
            rule = new RedirectToCanonicalHostRule(urlConfig);
        }

        [Test]
        public void DoesNotModifyMatchingHost()
        {
            //Arrange
            RewriteContext context = new RewriteContext();
            context.Result = RuleResult.ContinueRules;
            HttpContext httpContext= new DefaultHttpContext();
            
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString(urlConfig.CanonicalHost);
            httpContext.Request.PathBase = null;
            httpContext.Request.Path = new PathString("/primary/secondary");
            httpContext.Request.QueryString = new QueryString("?Foo=Bar");

            context.HttpContext = httpContext;

            //Act
            rule.ApplyRule(context);

            //Assert
            Assert.AreEqual(string.Empty, context.HttpContext.Response.Headers[HeaderNames.Location].ToString());
            Assert.AreEqual((int)HttpStatusCode.OK, context.HttpContext.Response.StatusCode);
            Assert.AreEqual(RuleResult.ContinueRules, context.Result);
        }

        [Test]
        public void ModifiesHostThatDoesntMatch()
        {
            //Arrange
            RewriteContext context = new RewriteContext();
            context.Result = RuleResult.ContinueRules;
            HttpContext httpContext = new DefaultHttpContext();

            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("www2.foo.bar");
            httpContext.Request.PathBase = null;
            httpContext.Request.Path = new PathString("/primary/secondary");
            httpContext.Request.QueryString = new QueryString("?Foo=Bar");

            context.HttpContext = httpContext;

            //Act
            rule.ApplyRule(context);

            //Assert
            Assert.AreEqual("https://www.foo.bar/primary/secondary?Foo=Bar", context.HttpContext.Response.Headers[HeaderNames.Location].ToString());
            Assert.AreEqual((int)HttpStatusCode.MovedPermanently, context.HttpContext.Response.StatusCode);
            Assert.AreEqual(RuleResult.EndResponse, context.Result);
        }
    }
}
