using Sitecore.Pipelines.HttpRequest;

namespace Skillcore.Stats.Processors
{
    public class SaveOutputCacheStats
    {
        public void Process(HttpRequestArgs args)
        {
            // it's a '/api/sitecore' request but 'StatsFilter.OnResultExecuted' method was not called
            // exception thrown or OutputCache used
            if (args.HttpContext.Response.StatusCode >= 400)
            {
                StatsCollector.SaveRequestError();
            }
            else
            {
                StatsCollector.SaveRequestStatistics(true);
            }
        }
    }
}
