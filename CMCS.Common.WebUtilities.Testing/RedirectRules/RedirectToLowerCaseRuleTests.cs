using CMCS.Common.WebUtilities.Objects;
using CMCS.Common.WebUtilities.RedirectRules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CMCS.Common.WebUtilities.Testing.RedirectRules
{
    public class RedirectToLowerCaseRuleTests
    {
        private RedirectToLowerCaseRule rule;

        [SetUp]
        public void Setup()
        {
            rule = new RedirectToLowerCaseRule();
        }

        [Test]
        public void DoesNotModifyLowerCaseUrl()
        {
            //Arrange
            RewriteContext context = new RewriteContext();
            context.Result = RuleResult.ContinueRules;
            HttpContext httpContext = new DefaultHttpContext();

            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("www.foo.bar");
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
        public void ModifiesNonLowerCaseUrl()
        {
            //Arrange
            RewriteContext context = new RewriteContext();
            context.Result = RuleResult.ContinueRules;
            HttpContext httpContext = new DefaultHttpContext();

            httpContext.Request.Scheme = "httPs";
            httpContext.Request.Host = new HostString("www.FoO.bar");
            httpContext.Request.PathBase = null;
            httpContext.Request.Path = new PathString("/pRiMaRy/SeCoNdary");
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
