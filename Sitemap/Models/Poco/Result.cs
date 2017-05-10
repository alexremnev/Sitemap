namespace Sitemap.Models.Poco
{
    public class Result
    {
        public Result()
        {
        }

        public Result(string pageUrl, double responseTime, string statusCode)
        {
            PageUrl = pageUrl;
            ResponseTime = responseTime;
            StatusCode = statusCode;
        }

        public string PageUrl { get; set; }
        public double ResponseTime { get; set; }
        public double BestTime { get; set; }
        public double WorstTime { get; set; }
        public string StatusCode { get; set; }
    }
}