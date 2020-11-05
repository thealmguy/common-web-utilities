using CMCS.Common.WebUtilities.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMCS.Common.WebUtilities.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "asp-path-match,asp-part-path-match")]
    public class ActivePathSegmentTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public ActivePathSegmentTagHelper(IHttpContextAccessor contextAccessor, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _contextAccessor = contextAccessor;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        [HtmlAttributeName("asp-part-path-match")]
        public bool FuzzySegmentMatch { get; set; }

        [HtmlAttributeName("asp-area")]
        public string Area { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (ShouldBeActive())
                MakeActive(output);
            
            output.Attributes.RemoveAll("asp-path-match");
            output.Attributes.RemoveAll("asp-part-path-match");
        }

        private string ConstructUrl(string[] parts, int countToConstruct)
        {
            int counter = 0;
            string ret = string.Empty;
            while (counter < countToConstruct)
            {
                ret += parts[counter] + "/";
                counter++;
            }

            ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }

        private bool ShouldBeActive()
        {
            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items.Where(ad => ad.AttributeRouteInfo != null).Select(ad => new RouteInformation(ad)).ToList();

            bool isMatch = false;

            if (FuzzySegmentMatch)
            {              
                var matchingRoutes = new List<RouteInformation>();
                var pathSegments = _contextAccessor.HttpContext.Request.Path.ToString().Substring(1).Split('/');
                if (pathSegments.Length > 1)
                {
                    for (int i = 1; i < pathSegments.Length; i++)
                    {
                        matchingRoutes.AddRange(routes.Where(r => r.Template == ConstructUrl(pathSegments, i)).ToList());
                    }
                }
                isMatch =  matchingRoutes.Any(r => r.Area == Area && r.Controller == Controller && r.Action == Action);
            }

            if (isMatch)
                return true;
            else
            {
                var route = routes.SingleOrDefault(r => r.Template == _contextAccessor.HttpContext.Request.Path.ToString().Substring(1));
                return route != null && route.Area == Area && route.Controller == Controller && route.Action == Action;
            }       
        }

        private void MakeActive(TagHelperOutput output)
        {
            var classAttr = output.Attributes.FirstOrDefault(a => a.Name == "class");
            if (classAttr == null)
            {
                classAttr = new TagHelperAttribute("class", "active");
                output.Attributes.Add(classAttr);
            }
            else if (classAttr.Value == null || classAttr.Value.ToString().IndexOf("active") < 0)
            {
                output.Attributes.SetAttribute("class", classAttr.Value == null ? "active" : classAttr.Value.ToString() + " active");
            }
        }









    }
}
