using log4net;
using Nancy;

namespace Kontur.GameStats.Server.Infrastructure
{
    public abstract class ModuleBase : NancyModule
    {
        protected readonly ILog Log;

        protected ModuleBase()
        {
            Log = LogManager.GetLogger(GetType());
            Before += ctx =>
            {
                Log.Info($"{ctx.Request.Url}");
                return null;
            };
            OnError += (ctx, ex) =>
            {
                Log.Error(ex);
                return null;
            };
        }
    }
}