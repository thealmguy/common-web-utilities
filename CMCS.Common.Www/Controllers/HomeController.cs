﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CMCS.Common.Www.Models;

namespace CMCS.Common.Www.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        #region "path segment example pages"

        [Route("foo")]
        public IActionResult TopLevel()
        {
            return View();
        }

        [Route("foo/bar")]
        public IActionResult Secondary()
        {
            return View();
        }

        [Route("foo/bar/ipsum")]
        public IActionResult Tertiary()
        {
            return View();
        }

        [Route("foo/bar/ipsum/lorem")]
        public IActionResult Quadrtiaryyyy()
        {
            return View();
        }

        #endregion

        #region "route dictionary examples"

        [Route("bar/{category}")]
        public IActionResult Category(string category)
        {
            return View("Generic", category);
        }

        [Route("bar/{category}/{subCategory}")]
        public IActionResult Subcategory(string category, string subCategory)
        {
            return View("Generic", $"{category} > {subCategory}");
        }

        [Route("bar/{category}/{subCategory}/{subSubCategory}")]
        public IActionResult SubSubcategory(string category, string subCategory, string subSubCategory)
        {
            return View("Generic", $"{category} > {subCategory} > {subSubCategory}");
        }

        [Route("bar/{category}/{subCategory}/{subSubCategory}/{subSubSubCategory}")]
        public IActionResult SubSubSubcategory(string category, string subCategory, string subSubCategory, string subSubSubCategory)
        {
            return View("Generic", $"{category} > {subCategory} > {subSubCategory} > {subSubSubCategory}");
        }

        [Route("bar/{category}/{subCategory}/{subSubCategory}/{subSubSubCategory}/{subSubSubSubCategory}")]
        public IActionResult SubSubSubSubcategory(string category, string subCategory, string subSubCategory, string subSubSubCategory, string subSubSubSubCategory)
        {
            return View("Generic", $"{category} > {subCategory} > {subSubCategory} > {subSubSubCategory} > {subSubSubSubCategory}");
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
