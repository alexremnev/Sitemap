using System.Collections.Generic;
using Sitemap.Models;
using Sitemap.Models.Poco;

namespace Sitemap.Service
{
    public interface IEngine
    {
        IList<Result> GetStatistic(string url);
        //for dynamic data
        IList<Result> GetDynamicStatistic(string url,int offset);
        IList<string> FindAllPageUrls(string path);
    }
}