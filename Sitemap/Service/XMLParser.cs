using System;
using System.Collections.Generic;
using System.Resources;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Common.Logging;

namespace Sitemap.Service
{
    public class XMLParser : IParser
    {
        private static readonly ILog Log = LogManager.GetLogger<XMLParser>();

        private const string Preffix = "sitemap";

        private const string XPathExpressionForIndexFile =
            "/" + Preffix + ":sitemapindex/" + Preffix + ":sitemap/" + Preffix + ":loc";

        private const string XPathExpressionForSitemapFile =
            "/" + Preffix + ":urlset/" + Preffix + ":url/" + Preffix + ":loc";

        private const string Schema = "http://www.sitemaps.org/schemas/sitemap/0.9";

        private static readonly IList<string> SitemapFiles = new List<string>
        {
            "sitemap.xml",
            "index.xml",
            "sitemapindex.xml"
        };

        public IEnumerable<string> GetUrlsFromSitemapFile(string url)
        {
            try
            {
                var urls = new List<string>();
                XmlReader xmlReader = url.Contains(".xml")
                    ? new XmlTextReader(url)
                    : new XmlTextReader($"{url}/sitemap.xml");
                var document = new XPathDocument(xmlReader);
                var navigator = document.CreateNavigator();
                var resolver = new XmlNamespaceManager(xmlReader.NameTable);
                resolver.AddNamespace(Preffix, Schema);
                var iterator = navigator.Select(XPathExpressionForSitemapFile, resolver);
                while (iterator.MoveNext())
                {
                    if (iterator.Current != null)
                        urls.Add(iterator.Current.Value);
                }
                return urls;
            }
            catch (Exception e)
            {
                Log.Error("Exception occured while processing sitemap file", e);
                throw;
            }
        }

        public IEnumerable<string> GetSitemapUrlFromIndexFile(string url)
        {
            var sitemapUrls = new List<string>();
            XmlReader xmlReader = new XmlTextReader($"{url}/sitemap.xml");
            var resolver = new XmlNamespaceManager(xmlReader.NameTable);
            var document = new XmlDocument();
            document.Load(xmlReader);
            resolver.AddNamespace(Preffix, Schema);
            var nodeList = document.SelectNodes(XPathExpressionForIndexFile, resolver);
            foreach (XmlNode node in nodeList)
            {
                sitemapUrls.Add(node.InnerText);
            }
            return sitemapUrls;
        }
    }
}