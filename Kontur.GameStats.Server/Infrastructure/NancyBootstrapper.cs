using Nancy;
using Nancy.TinyIoc;

namespace Kontur.GameStats.Server.Infrastructure
{
    internal class NancyBootstrapper : DefaultNancyBootstrapper
    {
        public TinyIoCContainer GetContainer()
        {
            return ApplicationContainer;
        }
    }
}