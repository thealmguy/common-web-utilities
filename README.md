# Common-Web-Utilities
A collection of helpers and utilities for asp.net core sites

There is a sample web project included in this repo. Simply clone and run the project to see a working implementation.

For help and support, please open an issue.

# Tag Helpers

## ActiveRoute TagHelper

### How to use it

- Install [this nuget package][1] into your asp.net project
- To your `View > _ViewImports.cshtml` add:
```
@addTagHelper *, CMCS.Common.WebUtilities
```
- To the `a` tag you want to apply the active class, add `asp-is-active` like so:
```
<a asp-is-active asp-area="" asp-controller="Home" asp-action="Index">Home</a>
```

### How it works
Using the ViewContext of the current request, the tag helper will check the `area`, `controller`, `action` and `route data` values against those specified on the `a` tag. If they match, the `active` class will be applied.

## ActivePathSegment TagHelper

### How to use it
- Install [this nuget package][1] into your asp.net project
- To your `View > _ViewImports.cshtml` add:
```
@addTagHelper *, CMCS.Common.WebUtilities
```
- To the `a` tag you want to apply the `active` class, add `asp-path-match`, and `asp-part-path-match` set to `false` if you only want it to show as active on and exact url match, or `true` if you want it to show as active on a partial path match. This latter example is useful when showing top level menus as active, even if it is actually a subpage that is being viewed.

 like so:
```
<a asp-path-match asp-part-path-match="false" asp-area="" asp-controller="Home" asp-action="Index">Home</a>
```
### How it works
The taghelper gets the current url path from `HttpContext`. If you have selected `true` for a partial match, it will split the path into its segments and calculate all possible paths to match against.

It then gets the routes that match this `area`, `controller` and `action`, and compares the calculated url against the previous list. If there is a match, it adds the `active` class.

# Services

## Sitemap Service

### How to use it

Docs coming soon! See sample project (Home controller > sitemap action)




<!-- - Email TagHelper

# Redirect and Rewrite Rules
- Redirect to Lowercase
- Redirect to canonical url -->


[1]:https://www.nuget.org/packages/CMCS.Common.WebUtilities