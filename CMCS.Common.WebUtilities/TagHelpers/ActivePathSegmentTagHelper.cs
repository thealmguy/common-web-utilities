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

        private IDictionary<string, string> _routeValues;

        [HtmlAttributeName("asp-part-path-match")]
        public bool FuzzySegmentMatch { get; set; }

        [HtmlAttributeName("asp-area")]
        public string Area { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

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

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (ShouldBeActive())
                MakeActive(output);
            
            output.Attributes.RemoveAll("asp-path-match");
            output.Attributes.RemoveAll("asp-part-path-match");
        }

        private List<string> GetPossibleUrls()
        {
            var path = _contextAccessor.HttpContext.Request.Path.ToString().Substring(1);
            if (FuzzySegmentMatch)
            {
                var ret = new List<string>();
                var pathSegments = path.Split("/");
                for(int i = 0; i< pathSegments.Length; i++)
                {
                    var potentialPath = string.Empty;
                    for(int pathSegment = 0; pathSegment <= i; pathSegment++)
                    {
                        potentialPath = pathSegment == 0 ? pathSegments[pathSegment] : string.Join("/", potentialPath, pathSegments[pathSegment]);
                    }
                    ret.Add(potentialPath);
                }
                return ret;
            }
            else
            {
                return new List<string>() { path };
            }
        }

        private void CompleteTemplate(RouteInformation routeInformation)
        {
            string completedUrl = string.Empty;
            foreach(string pathSegment in routeInformation.Template.Split("/"))
            {
                string finalPathSegment = string.Empty;
                if (pathSegment.StartsWith("{") && pathSegment.EndsWith("}") && RouteValues != null && RouteValues.Any())
                {
                    string routeDataKey = pathSegment.Replace("{", "").Replace("}", "");
                    if (!RouteValues.TryGetValue(routeDataKey, out finalPathSegment))
                        finalPathSegment = pathSegment;
                }
                else
                    finalPathSegment = pathSegment;
                completedUrl = completedUrl == string.Empty ? finalPathSegment :  string.Join('/', completedUrl, finalPathSegment);
            }
            routeInformation.SetTemplate(completedUrl);
        }

        private bool ShouldBeActive()
        {
            var urls = GetPossibleUrls();

            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                .Where(ad => ad.AttributeRouteInfo != null)
                .Select(ad => new RouteInformation(ad)).Where(ri => ri.Area.ToLowerInvariant() == Area.ToLowerInvariant() && ri.Controller.ToLowerInvariant() == Controller.ToLowerInvariant() && ri.Action.ToLowerInvariant() == Action.ToLowerInvariant()).ToList();
            routes.ForEach(CompleteTemplate);
            return routes.Any(r => urls.Contains(r.Template));    
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
