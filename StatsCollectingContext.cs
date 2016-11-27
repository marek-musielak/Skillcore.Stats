using System;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;

namespace Skillcore.Stats
{
    public class StatsCollectingContext : IDisposable
    {
        private static readonly ID DeviceTemplateId = ID.Parse("{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}");
        private StatsCollector _statsCollector;
        public bool UsedCache { get; set; }

        public StatsCollectingContext(string traceName, ID id)
        {
            if (StatsCollector.CollectStats)
            {
                _statsCollector = new StatsCollector(traceName, id);
            }
        }

        public StatsCollectingContext(RenderRenderingArgs args)
        {
            if (StatsCollector.CollectStats)
            {
                string traceName = GetRenderingNameOrUrl(args);

                Item item = args.Rendering.Item;
                _statsCollector = new StatsCollector(traceName, item != null ? item.ID : null);
            }
        }

        public void Dispose()
        {
            if (_statsCollector != null)
            {
                _statsCollector.SaveStatistics(UsedCache);
                _statsCollector = null;
            }
        }

        private static string GetRenderingNameOrUrl(RenderRenderingArgs args)
        {
            if (args.Rendering.RenderingItem.InnerItem.TemplateID == DeviceTemplateId)
            {
                Uri url = args.PageContext.RequestContext.HttpContext.Request.Url;
                if (url != null)
                {
                    return url.AbsolutePath;
                }
                return args.PageContext.RequestContext.HttpContext.Request.RawUrl;
            }

            return args.Rendering.RenderingItem.Name;
        }
    }
}
