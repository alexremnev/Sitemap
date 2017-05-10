using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitemap.Service
{
    public interface IParser
    {
         IEnumerable<string> GetUrlsFromSitemapFile(string url);
         IEnumerable<string> GetSitemapUrlFromIndexFile(string url);
    }
}