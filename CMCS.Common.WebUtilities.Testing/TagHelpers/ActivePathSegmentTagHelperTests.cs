using CMCS.Common.WebUtilities.TagHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMCS.Common.WebUtilities.Testing.TagHelpers
{
    public class ActivePathSegmentTagHelperTests
    {
        private ActivePathSegmentTagHelper helper;
        private TagHelperContext tagHelperContext;
        private TagHelperOutput tagHelperOutput;
        private IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;
        private IHttpContextAccessor contextAccessor;


        [SetUp]
        public void Setup()
        {
            contextAccessor = new HttpContextAccessor();
            contextAccessor.HttpContext = new DefaultHttpContext();
            
            List<ActionDescriptor> actionDescriptors = new List<ActionDescriptor>(){ 
                new ActionDescriptor(){ 
                    RouteValues = new Dictionary<string, string>(){{ "Area", "" }, {"Controller", "Home" }, {"Action", "Index" }}, 
                    AttributeRouteInfo = new Microsoft.AspNetCore.Mvc.Routing.AttributeRouteInfo(){ Name = "Home", Template = ""} 
                },
                new ActionDescriptor(){
                    RouteValues = new Dictionary<string, string>(){{ "Area", "" }, {"Controller", "Home" }, {"Action", "Primary" }},
                    AttributeRouteInfo = new Microsoft.AspNetCore.Mvc.Routing.AttributeRouteInfo(){ Name = "Home", Template = "primary"}
                },
                new ActionDescriptor(){
                    RouteValues = new Dictionary<string, string>(){{ "Area", "" }, {"Controller", "Home" }, {"Action", "Secondary" }},
                    AttributeRouteInfo = new Microsoft.AspNetCore.Mvc.Routing.AttributeRouteInfo(){ Name = "Home", Template = "primary/secondary"}
                },
                new ActionDescriptor(){
                    RouteValues = new Dictionary<string, string>(){{ "Area", "" }, {"Controller", "Home" }, {"Action", "Tertiary" }},
                    AttributeRouteInfo = new Microsoft.AspNetCore.Mvc.Routing.AttributeRouteInfo(){ Name = "Home", Template = "primary/secondary/tertiary"}
                },
                new ActionDescriptor(){
                    RouteValues = new Dictionary<string, string>(){{ "Area", "" }, {"Controller", "Home" }, {"Action", "Quadrary" }},
                    AttributeRouteInfo = new Microsoft.AspNetCore.Mvc.Routing.AttributeRouteInfo(){ Name = "Home", Template = "primary/secondary/tertiary/{aThing}"}
                }

            };
            
            actionDescriptorCollectionProvider = Substitute.For<IActionDescriptorCollectionProvider>();
            actionDescriptorCollectionProvider.ActionDescriptors.Returns(new ActionDescriptorCollection(actionDescriptors, 1));

            helper = new ActivePathSegmentTagHelper(contextAccessor, actionDescriptorCollectionProvider);
         

            tagHelperContext = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));

            tagHelperOutput = new TagHelperOutput("a", new TagHelperAttributeList() { new TagHelperAttribute("asp-path-match"), new TagHelperAttribute("asp-part-path-match") },
            (result, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });
        }

        [Test]
        public void AddsActiveClassForMatchingUrlWithNoSegments()
        {
            //Arrange
            contextAccessor.HttpContext.Request.Path = "/";
            helper.FuzzySegmentMatch = false;
            helper.Area = "";
            helper.Controller = "Home";
            helper.Action = "Index";

            //Act
            helper.Process(tagHelperContext, tagHelperOutput);

            //Assert
            TagHelperAttribute classAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("class", out classAttribute);
            Assert.AreEqual("active", classAttribute.Value);

            TagHelperAttribute aspActiveAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("asp-path-match", out aspActiveAttribute);
            Assert.IsNull(aspActiveAttribute);

            TagHelperAttribute aspOtherActiveAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("asp-part-path-match", out aspOtherActiveAttribute);
            Assert.IsNull(aspOtherActiveAttribute);
        }

        [Test]
        public void AddsActiveClassForMatchingUrlWithOneSegment()
        {
            //Arrange
            contextAccessor.HttpContext.Request.Path = "/primary";
            helper.FuzzySegmentMatch = false;
            helper.Area = "";
            helper.Controller = "Home";
            helper.Action = "Primary";

            //Act
            helper.Process(tagHelperContext, tagHelperOutput);

            //Assert
            TagHelperAttribute classAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("class", out classAttribute);
            Assert.AreEqual("active", classAttribute.Value);
        }

        [Test]
        public void AddsActiveClassForMatchingUrlWithTwoSegments()
        {
            //Arrange
            contextAccessor.HttpContext.Request.Path = "/primary/secondary";
            helper.FuzzySegmentMatch = false;
            helper.Area = "";
            helper.Controller = "Home";
            helper.Action = "Secondary";

            //Act
            helper.Process(tagHelperContext, tagHelperOutput);

            //Assert
            TagHelperAttribute classAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("class", out classAttribute);
            Assert.AreEqual("active", classAttribute.Value);
        }

        [Test]
        public void AddsActiveClassForMatchingUrlWithThreeSegments()
        {
            //Arrange
            contextAccessor.HttpContext.Request.Path = "/primary/secondary/tertiary";
            helper.FuzzySegmentMatch = false;
            helper.Area = "";
            helper.Controller = "Home";
            helper.Action = "Tertiary";

            //Act
            helper.Process(tagHelperContext, tagHelperOutput);

            //Assert
            TagHelperAttribute classAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("class", out classAttribute);
            Assert.AreEqual("active", classAttribute.Value);
        }

        [Test]
        public void AddsActiveClassForMatchingUrlWithThreeSegmentsFuzzyMatch()
        {
            //Arrange
            contextAccessor.HttpContext.Request.Path = "/primary/secondary/tertiary";
            helper.FuzzySegmentMatch = true;
            helper.Area = "";
            helper.Controller = "Home";
            helper.Action = "Primary";

            //Act
            helper.Process(tagHelperContext, tagHelperOutput);

            //Assert
            TagHelperAttribute classAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("class", out classAttribute);
            Assert.AreEqual("active", classAttribute.Value);
        }

        [Test]
        public void AddsActiveClassForMatchingUrlWithRouteData()
        {
            //Arrange
            contextAccessor.HttpContext.Request.Path = "/primary/secondary/tertiary/foobar";
            helper.FuzzySegmentMatch = false;
            helper.Area = "";
            helper.Controller = "Home";
            helper.Action = "Quadrary";
            helper.RouteValues = new Dictionary<string, string>();
            helper.RouteValues.Add("aThing", "foobar");

            //Act
            helper.Process(tagHelperContext, tagHelperOutput);

            //Assert
            TagHelperAttribute classAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("class", out classAttribute);
            Assert.AreEqual("active", classAttribute.Value);
        }
    }
}
