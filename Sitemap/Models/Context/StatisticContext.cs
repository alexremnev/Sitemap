using System.Data.Entity;
using Sitemap.Models.Poco;

namespace Sitemap.Models.Context
{
    public class StatisticContext : DbContext
    {
        public StatisticContext() : base("SitePerformance")
        {
//            Database.SetInitializer(new DropCreateDatabaseAlways<StatisticContext>());
//            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<StatisticContext>());
        }

        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<History> TimeResults { get; set; }
    }
}