using CMCS.Common.WebUtilities.Objects;
using CMCS.Common.WebUtilities.Services;
using CMCS.Common.WebUtilities.TagHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;

namespace CMCS.Common.WebUtilities.Testing.TagHelpers
{
    public class SitemapServiceTests
    {
        private SitemapService svc;

        [SetUp]
        public void Setup()
        {
            svc = new SitemapService();
        }

        [Test]
        public void ReturnsEntryWithExpectedValues()
        {
            //Arrange
            List<SitemapEntry> entries = new List<SitemapEntry>();
            entries.Add(new SitemapEntry("http://foo.bar", new DateTime(2020, 1, 1, 15, 00, 00), SitemapEntry.ChangeFrequencies.Monthly, 0.5m));

            //Act
            var sitemap = svc.GenerateSitemap(entries);

            //Assert
            Assert.AreEqual("<?xml version=\"1.0\"?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:image=\"http://www.google.com/schemas/sitemap-image/1.1\"><url><loc>http://foo.bar</loc><lastmod>2020-01-01</lastmod><changefreq>monthly</changefreq><priority>0.5</priority></url></urlset>", sitemap.InnerXml);
        }

        [Test]
        public void ReturnsEntryWithExpectedValuesWithRelativeUrls()
        {
            //Arrange
            var urlConfig = new UrlConfig();
            urlConfig.CanonicalHost = "foo.bar";
            urlConfig.HttpPort = 80;
            urlConfig.HttpsPort = 443;
            List<SitemapEntry> entries = new List<SitemapEntry>();
            entries.Add(new SitemapEntry("/travel", new DateTime(2020, 1, 1, 15, 00, 00), SitemapEntry.ChangeFrequencies.Monthly, 0.5m));

            //Act
            var sitemap = svc.GenerateSitemap(entries, urlConfig);

            //Assert
            Assert.AreEqual("<?xml version=\"1.0\"?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:image=\"http://www.google.com/schemas/sitemap-image/1.1\"><url><loc>https://foo.bar/travel</loc><lastmod>2020-01-01</lastmod><changefreq>monthly</changefreq><priority>0.5</priority></url></urlset>", sitemap.InnerXml);
        }

        [Test]
        public void ReturnsEntryWithExpectedValuesWithActionDescriptorCollection()
        {
            //Arrange
            var urlConfig = new UrlConfig();
            urlConfig.CanonicalHost = "foo.bar";
            urlConfig.HttpPort = 80;
            urlConfig.HttpsPort = 443;
            List<ActionDescriptor> actionDescriptors = new List<ActionDescriptor>();
            actionDescriptors.Add(new ActionDescriptor() { AttributeRouteInfo = new AttributeRouteInfo() { Template = "foo/bar" } });

            //Act
            var sitemap = svc.GenerateSitemap(new ActionDescriptorCollection(actionDescriptors, 1), urlConfig);

            //Assert
            Assert.AreEqual("<?xml version=\"1.0\"?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:image=\"http://www.google.com/schemas/sitemap-image/1.1\"><url><loc>https://foo.bar/foo/bar</loc></url></urlset>", sitemap.InnerXml);
        }

        [Test]
        public void ReturnsEntryWithExpectedValuesWithRelativeUrlsAndFunkyHttpsPort()
        {
            //Arrange
            var urlConfig = new UrlConfig();
            urlConfig.CanonicalHost = "foo.bar";
            urlConfig.HttpPort = 80;
            urlConfig.HttpsPort = 53443;
            List<SitemapEntry> entries = new List<SitemapEntry>();
            entries.Add(new SitemapEntry("/travel", new DateTime(2020, 1, 1, 15, 00, 00), SitemapEntry.ChangeFrequencies.Monthly, 0.5m));

            //Act
            var sitemap = svc.GenerateSitemap(entries, urlConfig);

            //Assert
            Assert.AreEqual("<?xml version=\"1.0\"?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:image=\"http://www.google.com/schemas/sitemap-image/1.1\"><url><loc>https://foo.bar:53443/travel</loc><lastmod>2020-01-01</lastmod><changefreq>monthly</changefreq><priority>0.5</priority></url></urlset>", sitemap.InnerXml);
        }

        [Test]
        public void ReturnsEntryWithExpectedValuesWithRelativeUrlsAndHttpPort()
        {
            //Arrange
            var urlConfig = new UrlConfig();
            urlConfig.CanonicalHost = "foo.bar";
            urlConfig.HttpPort = 80;
            urlConfig.HttpsPort = null;
            List<SitemapEntry> entries = new List<SitemapEntry>();
            entries.Add(new SitemapEntry("/travel", new DateTime(2020, 1, 1, 15, 00, 00), SitemapEntry.ChangeFrequencies.Monthly, 0.5m));

            //Act
            var sitemap = svc.GenerateSitemap(entries, urlConfig);

            //Assert
            Assert.AreEqual("<?xml version=\"1.0\"?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:image=\"http://www.google.com/schemas/sitemap-image/1.1\"><url><loc>http://foo.bar/travel</loc><lastmod>2020-01-01</lastmod><changefreq>monthly</changefreq><priority>0.5</priority></url></urlset>", sitemap.InnerXml);
        }

        [Test]
        public void ReturnsEntryWithExpectedValuesWithRelativeUrlsAndFunkyHttpPort()
        {
            //Arrange
            var urlConfig = new UrlConfig();
            urlConfig.CanonicalHost = "foo.bar";
            urlConfig.HttpPort = 8080;
            urlConfig.HttpsPort = null;
            List<SitemapEntry> entries = new List<SitemapEntry>();
            entries.Add(new SitemapEntry("/travel", new DateTime(2020, 1, 1, 15, 00, 00), SitemapEntry.ChangeFrequencies.Monthly, 0.5m));

            //Act
            var sitemap = svc.GenerateSitemap(entries, urlConfig);

            //Assert
            Assert.AreEqual("<?xml version=\"1.0\"?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:image=\"http://www.google.com/schemas/sitemap-image/1.1\"><url><loc>http://foo.bar:8080/travel</loc><lastmod>2020-01-01</lastmod><changefreq>monthly</changefreq><priority>0.5</priority></url></urlset>", sitemap.InnerXml);
        }

        [Test]
        public void ReturnsEntryWithExpectedValuesWithRelativeUrlsAndNoPorts()
        {
            //Arrange
            var urlConfig = new UrlConfig();
            urlConfig.CanonicalHost = "foo.bar";
            List<SitemapEntry> entries = new List<SitemapEntry>();
            entries.Add(new SitemapEntry("/travel", new DateTime(2020, 1, 1, 15, 00, 00), SitemapEntry.ChangeFrequencies.Monthly, 0.5m));

            //Act
            var sitemap = svc.GenerateSitemap(entries, urlConfig);

            //Assert
            Assert.AreEqual("<?xml version=\"1.0\"?><urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:image=\"http://www.google.com/schemas/sitemap-image/1.1\"><url><loc>http://foo.bar/travel</loc><lastmod>2020-01-01</lastmod><changefreq>monthly</changefreq><priority>0.5</priority></url></urlset>", sitemap.InnerXml);
        }
    }
}
