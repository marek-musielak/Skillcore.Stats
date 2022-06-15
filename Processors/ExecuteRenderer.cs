using Sitecore.Mvc.Pipelines.Response.RenderRendering;

namespace Skillcore.Stats.Processors
{
    public class ExecuteRenderer : Sitecore.Mvc.Pipelines.Response.RenderRendering.ExecuteRenderer
    {
        public ExecuteRenderer()
        {
        }

        public ExecuteRenderer(IRendererErrorStrategy errorStrategy) : base(errorStrategy)
        {

        }

        public override void Process(RenderRenderingArgs args)
        {
            using (StatsCollectingContext context = new StatsCollectingContext(args))
            {
                base.Process(args);
                context.UsedCache = args.UsedCache;
            }
        }
    }
}
