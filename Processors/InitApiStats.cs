using Sitecore.Pipelines.HttpRequest;

namespace Skillcore.Stats.Processors
{
    public class InitApiStats
    {
        public void Process(HttpRequestArgs args)
        {
            if (StatsCollector.CollectStats && StatsCollector.IsItApiCall)
            {
                StatsCollector.InitRequestStatsCollector(args.HttpContext);
            }
        }
    }
}
