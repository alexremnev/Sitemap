using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Castle.Core.Internal;
using Common.Logging;
using Sitemap.Service;

namespace Sitemap.Controllers
{
    public class StatisticController : Controller
    {
        private readonly IEngine _engine;
        private static readonly ILog Log = LogManager.GetLogger(typeof(StatisticController));

        public StatisticController(IEngine engine)
        {
            _engine = engine;
        }

        [HttpPost]
        public ActionResult Chart(string url)
        {
            try
            {
                if (!IsValidUri(url)) return RedirectToAction("Index", "Home");
                var results = _engine.GetStatistic(url);
                if (results.IsNullOrEmpty()) return RedirectToAction("Index", "Home");
                return View("Chart", results);
            }
            catch (Exception e)
            {
                Log.Error("Exceprion occured when app tried to render chart", e);
                return RedirectToAction("Error", "Home");
            }
        }

        private static bool IsValidUri(string url)
        {
            Uri outUri;
            return Uri.TryCreate(url, UriKind.Absolute, out outUri)
                   && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
        }


        //for dynamic results
        public ActionResult DynamicChart(string url)
        {
            try
            {
                if (!IsValidUri(url)) return RedirectToAction("Index", "Home");
                return View("DynamicChart", (object) url);
            }
            catch (Exception e)
            {
                Log.Error("Exceprion occured when app tried to render dynamic chart", e);
                return RedirectToAction("Error", "Home");
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetResult(string path, int offset)
        {
            var listList = new List<List<string>>();
            try
            {
                var results = _engine.GetDynamicStatistic(path, offset);
                var list = new List<string>
                {
                    offset.ToString(),
                    results[0].ResponseTime.ToString(CultureInfo.InvariantCulture),
                    results[0].PageUrl,
                    results[0].BestTime.ToString(CultureInfo.InvariantCulture),
                    results[0].WorstTime.ToString(CultureInfo.InvariantCulture),
                    results[0].StatusCode
                };
                if (offset == 0)
                {
                    list.Add(GetCountOfUrls(path).ToString());
                }
                listList.Add(list);
                return Json(listList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error("Exception occured while estimating site's response time", e);
                return Json(listList, JsonRequestBehavior.AllowGet);
            }
        }

        private int GetCountOfUrls(string path)
        {
            var pageUrls = _engine.FindAllPageUrls(path);
            return pageUrls.Count;
        }
    }
}