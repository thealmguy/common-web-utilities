using Microsoft.AspNetCore.Mvc.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CMCS.Common.WebUtilities.Objects
{
    public class RouteInformation
    {

        public RouteInformation(string template, string name, string area, string controller, string action)
        {
            this.Template = template;
            this.Name = name;
            this.Area = area;
            this.Controller = controller;
            this.Action = action;
        }

        public RouteInformation(ActionDescriptor routeInfo)
        {
            this.Template = routeInfo.AttributeRouteInfo.Template;
            this.Name = routeInfo.AttributeRouteInfo.Name;
            this.Area = routeInfo.RouteValues.ContainsKey("Area") ? routeInfo.RouteValues["Area"] : string.Empty;
            this.Controller = routeInfo.RouteValues.ContainsKey("Controller") ? routeInfo.RouteValues["Controller"] : string.Empty; ;
            this.Action = routeInfo.RouteValues.ContainsKey("Action") ? routeInfo.RouteValues["Action"] : string.Empty; ;

        }

        public string Template { get; private set; }

        public string Name { get; private set; }

        public string Area { get; private set; }

        public string Controller { get; private set; }

        public string Action { get; private set; }


        public void SetTemplate(string template)
        {
            this.Template = template;
        }

    }
}
