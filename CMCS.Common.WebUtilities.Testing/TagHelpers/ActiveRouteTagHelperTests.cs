using CMCS.Common.WebUtilities.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace CMCS.Common.WebUtilities.Testing.TagHelpers
{
    public class ActiveRouteTagHelperTests
    {
        private ActiveRouteTagHelper helper;
        private ViewContext viewContext;
        private TagHelperContext tagHelperContext;
        private TagHelperOutput tagHelperOutput;


        [SetUp]
        public void Setup()
        {
            helper = new ActiveRouteTagHelper();
            helper.Area = "";
            helper.Controller = "Home";
            helper.Action = "Index";
            helper.RouteValues = new Dictionary<string, string>();
            helper.RouteValues.Add("Id", "Foo");
            helper.RouteValues.Add("Name", "Bar");

            viewContext = new ViewContext();
            viewContext.RouteData = new RouteData();
            helper.ViewContext = viewContext;

            tagHelperContext = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));

            tagHelperOutput = new TagHelperOutput("a", new TagHelperAttributeList() { new TagHelperAttribute("asp-is-active") },
            (result, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetHtmlContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });
        }

        [Test]
        public void AddsActiveClassForMatchingRoute()
        {

            //Arrange
            viewContext.RouteData.Values.Add("Area", "");
            viewContext.RouteData.Values.Add("Controller", "Home");
            viewContext.RouteData.Values.Add("Action", "Index");
            viewContext.RouteData.Values.Add("Id", "Foo");
            viewContext.RouteData.Values.Add("Name", "Bar");

            //Act
            helper.Process(tagHelperContext, tagHelperOutput);

            //Assert
            TagHelperAttribute classAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("class", out classAttribute);
            Assert.AreEqual("active", classAttribute.Value);

            TagHelperAttribute aspActiveAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("asp-is-active", out aspActiveAttribute);
            Assert.IsNull(aspActiveAttribute);
        }

        [Test]
        public void DoesNotAddActiveClassNotMatchingArea()
        {

            //Arrange
            viewContext.RouteData.Values.Add("Area", "Fred");
            viewContext.RouteData.Values.Add("Controller", "Home");
            viewContext.RouteData.Values.Add("Action", "Index");
            viewContext.RouteData.Values.Add("Id", "Foo");
            viewContext.RouteData.Values.Add("Name", "Bar");

            //Act
            helper.Process(tagHelperContext, tagHelperOutput);

            //Assert
            TagHelperAttribute classAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("class", out classAttribute);
            Assert.IsNull(classAttribute);
        }

        [Test]
        public void DoesNotAddActiveClassNotMatchingController()
        {

            //Arrange
            viewContext.RouteData.Values.Add("Area", "");
            viewContext.RouteData.Values.Add("Controller", "Fred");
            viewContext.RouteData.Values.Add("Action", "Index");
            viewContext.RouteData.Values.Add("Id", "Foo");
            viewContext.RouteData.Values.Add("Name", "Bar");

            //Act
            helper.Process(tagHelperContext, tagHelperOutput);

            //Assert
            TagHelperAttribute classAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("class", out classAttribute);
            Assert.IsNull(classAttribute);
        }

        [Test]
        public void DoesNotAddActiveClassNotMatchingAction()
        {

            //Arrange
            viewContext.RouteData.Values.Add("Area", "");
            viewContext.RouteData.Values.Add("Controller", "Home");
            viewContext.RouteData.Values.Add("Action", "Fred");
            viewContext.RouteData.Values.Add("Id", "Foo");
            viewContext.RouteData.Values.Add("Name", "Bar");

            //Act
            helper.Process(tagHelperContext, tagHelperOutput);

            //Assert
            TagHelperAttribute classAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("class", out classAttribute);
            Assert.IsNull(classAttribute);
        }

        [Test]
        public void DoesNotAddActiveClassNotMatchingRouteData()
        {

            //Arrange
            viewContext.RouteData.Values.Add("Area", "");
            viewContext.RouteData.Values.Add("Controller", "Home");
            viewContext.RouteData.Values.Add("Action", "Index");
            viewContext.RouteData.Values.Add("Id", "Fred");
            viewContext.RouteData.Values.Add("Name", "Bar");

            //Act
            helper.Process(tagHelperContext, tagHelperOutput);

            //Assert
            TagHelperAttribute classAttribute = null;
            tagHelperOutput.Attributes.TryGetAttribute("class", out classAttribute);
            Assert.IsNull(classAttribute);
        }
    }
}
