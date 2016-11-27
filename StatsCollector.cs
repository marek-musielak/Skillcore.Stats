using System;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Diagnostics.PerformanceCounters;
using Sitecore.Data;

namespace Skillcore.Stats
{
    public class StatsCollector
    {
        public static bool CollectStats { get; private set; }
        private static string ApiPath { get; set; }
        public const string CollectorKey = "Skillcore.Stats.Collector";
        public const string ParentCollectorKey = "Skillcore.Stats.Parent.Collector";

        protected HighResTimer Timer;
        protected long ItemsAccessedBefore;
        protected string TraceName;
        protected string ParentName;
        protected bool UsedCache { get; set; }
        protected ID Id { get; set; }

        static StatsCollector()
        {
            CollectStats = Settings.GetBoolSetting("Collect.Performance.Stats", false);
            ApiPath = Settings.GetSetting("Skillcore.Stats.CommandRoutePrefix") ?? String.Empty;
            if (!ApiPath.StartsWith("/"))
            {
                ApiPath = "/" + ApiPath;
            }
        }

        public StatsCollector(string traceName, ID id = null)
        {
            if (CollectStats)
            {
                Timer = new HighResTimer(true);
                ItemsAccessedBefore = DataCounters.ItemsAccessed.Value;
                TraceName = traceName;
                Id = id;
                if (HttpContext.Current != null)
                {
                    ParentName = HttpContext.Current.Items[ParentCollectorKey] as string;
                    HttpContext.Current.Items[ParentCollectorKey] = String.Format("{0}[{1}]/", ParentName, TraceName);
                }
            }
        }

        public static bool IsItApiCall
        {
            get { return HttpContext.Current != null &&  HttpContext.Current.Request.RawUrl.ToLower().StartsWith(ApiPath); }
        }

        public virtual void SaveStatistics(bool usedCache)
        {
            if (CollectStats)
            {
                UsedCache = usedCache;
                SaveStatistics();
            }
        }

        public virtual void SaveStatistics()
        {
            if (CollectStats)
            {
                long itemsAccessed = DataCounters.ItemsAccessed.Value - ItemsAccessedBefore;
                double elapsed = Timer.Elapsed();
                Statistics.AddRenderingData(String.Format("{0}{1} {2} [Total]", ParentName, TraceName, Id), elapsed, itemsAccessed, UsedCache);
                Statistics.AddRenderingData(String.Format("{0}{1} {2} [{3}Cached]", ParentName, TraceName, Id, UsedCache ? "" : "Not "), elapsed, itemsAccessed, UsedCache);
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[ParentCollectorKey] = ParentName;
                }
            }
        }

        public virtual void SaveError()
        {
            if (CollectStats)
            {
                long itemsAccessed = DataCounters.ItemsAccessed.Value - ItemsAccessedBefore;
                Statistics.AddRenderingData(String.Format("{0}{1} {2} [Error]", ParentName, TraceName, Id), Timer.Elapsed(), itemsAccessed, UsedCache);
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[ParentCollectorKey] = ParentName;
                }
            }
        }

        public static void InitRequestStatsCollector(HttpContext context)
        {
            context.Items[CollectorKey] = new StatsCollector(context.Request.Url.AbsolutePath);
        }

        public static void SaveRequestStatistics(bool usedCache)
        {
            StatsCollector collector = HttpContext.Current != null ? HttpContext.Current.Items[CollectorKey] as StatsCollector : null;
            if (collector != null)
            {
                HttpContext.Current.Items[CollectorKey] = null;
                collector.SaveStatistics(usedCache);
            }
        }

        public static void SaveRequestError()
        {
            StatsCollector collector = HttpContext.Current != null ? HttpContext.Current.Items[CollectorKey] as StatsCollector : null;
            if (collector != null)
            {
                HttpContext.Current.Items[CollectorKey] = null;
                collector.SaveError();
            }
        }


    }
}
