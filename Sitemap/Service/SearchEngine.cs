using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Castle.Core.Internal;
using Common.Logging;
using Sitemap.Models.Poco;
using Sitemap.Models.Repository;

namespace Sitemap.Service
{
    public class SearchEngine : IEngine
    {
        public SearchEngine(IParser xmlParser, IStatisticRepository statisticRepository)
        {
            _xmlParser = xmlParser;
            _statisticRepository = statisticRepository;
        }

        private static readonly HttpClient client = new HttpClient();
        private readonly IParser _xmlParser;
        private readonly IStatisticRepository _statisticRepository;
        private static readonly ILog Log = LogManager.GetLogger<SearchEngine>();

        public IList<Result> GetStatistic(string path)
        {
            try
            {
                var urls = FindAllPageUrls(path);
                urls = urls.Count > 30 ? urls.Take(30).ToList() : urls;
                var estimateTime = EstimateTime(urls);
                foreach (var result in estimateTime)
                {
                    var urlHistory = _statisticRepository.Get(result.PageUrl);
                    if (urlHistory == null)
                    {
                        var statistic = new Statistic
                        {
                            PageUrl = result.PageUrl,
                            BestTime = result.ResponseTime,
                            WorstTime = result.ResponseTime,
                            HistoryResults = new List<History>()
                        };
                        var timeResult = new History()
                        {
                            PageUrl = result.PageUrl,
                            TimeResponse = result.ResponseTime
                        };
                        statistic.HistoryResults.Add(timeResult);
                        _statisticRepository.Create(statistic);
                    }
                    else
                    {
                        urlHistory.HistoryResults.Add(new History
                        {
                            TimeResponse = result.ResponseTime
                        });
                        var tupleResult = _statisticRepository.Update(urlHistory);
                        result.BestTime = tupleResult.Item1;
                        result.WorstTime = tupleResult.Item2;
                    }
                }
                return estimateTime;
            }
            catch (Exception e)
            {
                Log.Error("Exception occured while estimating site's response time", e);
                throw;
            }
        }

        public IList<string> FindAllPageUrls(string path)
        {
            try
            {
                var urls = new List<string>();
                urls.AddRange(GetUrlsFromIndexFile(path));
                if (urls.IsNullOrEmpty()) urls.AddRange(_xmlParser.GetUrlsFromSitemapFile(path));
                return urls;
            }
            catch (Exception e)
            {
                Log.Error("Exception occured when app tried to find all page urls", e);
                throw;
            }
        }

        private IList<string> GetUrlsFromIndexFile(string path)
        {
            var urls = new List<string>();
            var sitemapFiles = _xmlParser.GetSitemapUrlFromIndexFile(path).ToList();
            if (sitemapFiles.IsNullOrEmpty()) return urls;
            Parallel.ForEach(sitemapFiles,
                (sitemapUrl) => { urls.AddRange(_xmlParser.GetUrlsFromSitemapFile(sitemapUrl)); });
            return urls;
        }

        private static IList<Result> EstimateTime(IList<string> urls)
        {
            var result = new List<Result>();

            Parallel.ForEach(urls, url =>
            {
                var watch = Stopwatch.StartNew();
                var response = client.GetAsync(url);
                var statusCode = response.GetAwaiter().GetResult().StatusCode.ToString();
                watch.Stop();
                var pageUrl = UrlDecode(url);
                result.Add(new Result(pageUrl, watch.Elapsed.TotalSeconds,
                    statusCode));
            });

            return result.OrderByDescending(x => x.ResponseTime).ToList();
        }

        private static string UrlDecode(string url)
        {
            return HttpUtility.UrlDecode(url, Encoding.UTF8);
        }


//-------------------------------------for dynamic results

        public IList<Result> GetDynamicStatistic(string path, int offset)
        {
            try
            {
                var urls = FindAllPageUrls(path);
                urls = urls.Count > 30 ? urls.Take(30).ToList() : urls;
                var estimateTime = EstimateTime(urls.Skip(offset).Take(1).ToList());
                foreach (var result in estimateTime)
                {
                    var urlHistory = _statisticRepository.Get(result.PageUrl);
                    if (urlHistory == null)
                    {
                        var statistic = new Statistic
                        {
                            PageUrl = result.PageUrl,
                            BestTime = result.ResponseTime,
                            WorstTime = result.ResponseTime,
                            HistoryResults = new List<History>()
                        };
                        var timeResult = new History()
                        {
                            PageUrl = result.PageUrl,
                            TimeResponse = result.ResponseTime
                        };
                        statistic.HistoryResults.Add(timeResult);
                        _statisticRepository.Create(statistic);
                    }
                    else
                    {
                        urlHistory.HistoryResults.Add(new History
                        {
                            TimeResponse = result.ResponseTime
                        });
                        var tupleResult = _statisticRepository.Update(urlHistory);
                        result.BestTime = tupleResult.Item1;
                        result.WorstTime = tupleResult.Item2;
                    }
                }
                return estimateTime;
            }
            catch (Exception e)
            {
                Log.Error("Exception occured while estimating site's response time dynamic", e);
                throw;
            }
        }
    }
}