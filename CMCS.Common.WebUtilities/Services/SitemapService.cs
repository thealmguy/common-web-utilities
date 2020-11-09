using CMCS.Common.WebUtilities.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace CMCS.Common.WebUtilities.Services
{
    public class SitemapService
    {

        private XmlDocument CreateSitemap(List<SitemapEntry> sitemapEntries)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmldecl;
            xmldecl = doc.CreateXmlDeclaration("1.0", null, null);

            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmldecl, root);

            XmlNode urlset = doc.CreateNode(XmlNodeType.Element, "urlset", "");
            XmlAttribute att = doc.CreateAttribute("xmlns");
            att.Value = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XmlAttribute att2 = doc.CreateAttribute("xmlns:image");
            att2.Value = "http://www.google.com/schemas/sitemap-image/1.1";
            urlset.Attributes.Append(att);
            urlset.Attributes.Append(att2);

            sitemapEntries.ForEach(r =>
            {
                XmlNode node = doc.CreateNode(XmlNodeType.Element, "url", "");

                XmlNode loc = doc.CreateNode(XmlNodeType.Element, "loc", "");
                loc.InnerText = r.Location;
                node.AppendChild(loc);

                if (r.LastModified.HasValue)
                {
                    XmlNode lastMod = doc.CreateNode(XmlNodeType.Element, "lastmod", "");
                    lastMod.InnerText = r.LastModified.Value.ToString("yyyy-MM-dd");
                    node.AppendChild(lastMod);
                }

                if (r.ChangeFrequency.HasValue)
                {
                    XmlNode changeFrequency = doc.CreateNode(XmlNodeType.Element, "changefreq", "");
                    changeFrequency.InnerText = r.ChangeFrequency.Value.ToString().ToLower();
                    node.AppendChild(changeFrequency);
                }

                if (r.Priority.HasValue)
                {
                    XmlNode priority = doc.CreateNode(XmlNodeType.Element, "priority", "");
                    priority.InnerText = r.Priority.Value.ToString("0.#"); ;
                    node.AppendChild(priority);
                }
                urlset.AppendChild(node);
            });
            doc.AppendChild(urlset);
            return doc;
        }

        private HostString GetHost(UrlConfig urlConfig)
        {
            if (urlConfig.HttpsPort.HasValue)
            {
                if (urlConfig.HttpsPort.Value == 443 || urlConfig.HttpsPort.Value <= 0)
                    return new HostString(urlConfig.CanonicalHost);
                else
                    return new HostString(urlConfig.CanonicalHost, urlConfig.HttpsPort.Value);
            } 
            else if (urlConfig.HttpPort.HasValue)
            {
                if (urlConfig.HttpPort == 80 || urlConfig.HttpPort.Value <= 0)
                    return new HostString(urlConfig.CanonicalHost);
                else
                    return new HostString(urlConfig.CanonicalHost, urlConfig.HttpPort.Value); 
            }
            else
            {
                return new HostString(urlConfig.CanonicalHost);
            }
        }

        public XmlDocument GenerateSitemap(List<SitemapEntry> entries)
        {
            return CreateSitemap(entries);
        }

        public XmlDocument GenerateSitemap(List<SitemapEntry> entries, UrlConfig urlConfig)
        {
            HostString host = GetHost(urlConfig);

            entries.ForEach(e =>
            {
                if (e.Location.StartsWith("/"))
                    e.SetLocation(UriHelper.BuildAbsolute(urlConfig.HttpsPort.HasValue && urlConfig.HttpsPort > 0 ? "https" : "http",
                        host, path: e.Location));
            });
            return CreateSitemap(entries);
        }

        public XmlDocument GenerateSitemap(ActionDescriptorCollection actionDescriptorCollection, UrlConfig urlConfig)
        {
            HostString host = GetHost(urlConfig);
            List<SitemapEntry> entries = new List<SitemapEntry>();

            actionDescriptorCollection.Items.ToList().ForEach(ad =>
            {
                if (ad.AttributeRouteInfo != null && !ad.AttributeRouteInfo.Template.Contains("{") && !ad.AttributeRouteInfo.Template.Contains("}"))
                {
                    entries.Add(new SitemapEntry(UriHelper.BuildAbsolute(urlConfig.HttpsPort.HasValue && urlConfig.HttpsPort > 0 ? "https" : "http", host, path: "/" + ad.AttributeRouteInfo.Template)));
                }
            });
            return CreateSitemap(entries);
        }


    }
}
