using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Bugaboo.Startup))]
namespace Bugaboo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
