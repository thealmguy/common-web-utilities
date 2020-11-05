using CMCS.Common.WebUtilities.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMCS.Common.WebUtilities.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "asp-is-active")]
    public class ActiveRouteTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public ActiveRouteTagHelper(IHttpContextAccessor contextAccessor, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _contextAccessor = contextAccessor;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        private IDictionary<string, string> _routeValues;

        [HtmlAttributeName("asp-top-level-match")]
        public bool TopLevelMatch { get; set; }

        [HtmlAttributeName("asp-area")]
        public string Area { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-page")]
        public string Page { get; set; }

        [HtmlAttributeName("asp-all-route-data", DictionaryAttributePrefix = "asp-route-")]
        public IDictionary<string, string> RouteValues
        {
            get
            {
                if (_routeValues == null)
                    _routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                return _routeValues;
            }
            set
            {
                _routeValues = value;
            }
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (ShouldBeActive())
            {
                MakeActive(output);
            }

            output.Attributes.RemoveAll("asp-is-active");
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

            if (TopLevelMatch)
            {
                var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items.Where(ad => ad.AttributeRouteInfo != null).Select(ad => new RouteInformation(ad)).ToList();
                var matchingRoutes = new List<RouteInformation>();

                var pathSegments = _contextAccessor.HttpContext.Request.Path.ToString().Substring(1).Split('/');
                if (pathSegments.Length > 1)
                {
                    for (int i = 1; i < pathSegments.Length; i++)
                    {
                        var url = ConstructUrl(pathSegments, i);
                        matchingRoutes = routes.Where(r => r.Template == url).ToList();
                    }
                }
                return matchingRoutes.Any(r => r.Area == Area && r.Controller == Controller && r.Action == Action);             
            }

            string currentArea = string.Empty;
            string currentController = string.Empty;
            string currentAction = string.Empty;

            if (ViewContext.RouteData.Values["Area"] != null)
                currentArea = ViewContext.RouteData.Values["Area"].ToString();
            
            if (ViewContext.RouteData.Values["Controller"] != null)
                currentController = ViewContext.RouteData.Values["Controller"].ToString();
            
            if (ViewContext.RouteData.Values["Action"] != null)
                currentAction = ViewContext.RouteData.Values["Action"].ToString();

            if (Controller != null && Area != null)
            {
                if (!string.IsNullOrWhiteSpace(Area) && Area.ToLower() != currentArea.ToLower())
                    return false;
                
                if (!string.IsNullOrWhiteSpace(Controller) && Controller.ToLower() != currentController.ToLower())
                    return false;
                
                if (!string.IsNullOrWhiteSpace(Action) && Action.ToLower() != currentAction.ToLower())
                    return false;          
            }

            if (Page != null)
            {
                if (!string.IsNullOrWhiteSpace(Page) && Page.ToLower() != _contextAccessor.HttpContext.Request.Path.Value.ToLower())
                    return false;
                
            }

            foreach (KeyValuePair<string, string> routeValue in RouteValues)
            {
                if (!ViewContext.RouteData.Values.ContainsKey(routeValue.Key) ||
                    ViewContext.RouteData.Values[routeValue.Key].ToString() != routeValue.Value)
                    return false;
                
            }
            return true;
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
                output.Attributes.SetAttribute("class", classAttr.Value == null
                    ? "active" : classAttr.Value.ToString() + " active");
            }
        }
    }
}
