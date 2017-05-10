using System;
using System.Collections.Generic;
using Sitemap.Models.Poco;

namespace Sitemap.Models.Repository
{
    public interface IStatisticRepository
    {
        void Create(Statistic statistic);
        IList<Statistic> List();
        Statistic Get(string url);
        Tuple<double, double> Update(Statistic entity);
    }
}