using Sitecore.Mvc.Pipelines.MvcEvents.ResultExecuted;

namespace Skillcore.Stats.Processors
{
    public class SaveApiStats
    {
        public void Process(ResultExecutedArgs args)
        {
            StatsCollector.SaveRequestStatistics(false);
        }
    }
}
