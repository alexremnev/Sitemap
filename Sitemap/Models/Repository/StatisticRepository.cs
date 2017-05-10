using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Common.Logging;
using Sitemap.Models.Context;
using Sitemap.Models.Poco;

namespace Sitemap.Models.Repository
{
    public class StatisticRepository : IStatisticRepository
    {
        private static readonly ILog Log = LogManager.GetLogger<StatisticRepository>();

        public StatisticRepository()
        {
            _context = new StatisticContext();
            _context.Database.Log = message => Log.Debug(message);
        }

        private readonly StatisticContext _context;

        public void Create(Statistic statistic)
        {
            try
            {
                using (var context = new StatisticContext())
                {
                    statistic.BestTime = statistic.WorstTime = statistic.HistoryResults[0].TimeResponse;
                    context.Statistics.Add(statistic);
                    context.SaveChanges();
                    context.Database.Log = message => Log.Debug(message);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception occured while creating site statistics", e);
                throw;
            }
        }

        public IList<Statistic> List()
        {
            try
            {
                var context = new StatisticContext();
                context.Database.Log = message => Log.Debug(message);
                return context.Statistics.ToList();
            }
            catch (Exception e)
            {
                Log.Error("Exception occured while getting the list of site statistics", e);
                throw;
            }
        }

        public Statistic Get(string url)
        {
            try
            {
                return _context.Statistics.Find(url);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured while getting the site statistics by url", e);
                throw;
            }
        }

        public Tuple<double, double> Update(Statistic entity)
        {
            try
            {
                if (entity == null) throw new ArgumentNullException(nameof(entity));
                entity.BestTime = entity.HistoryResults.Max(x => x.TimeResponse);
                entity.WorstTime = entity.HistoryResults.Min(x => x.TimeResponse);
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();
                return new Tuple<double, double>(entity.BestTime, entity.WorstTime);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured while updating the site statistics", e);
                throw;
            }
        }
    }
}