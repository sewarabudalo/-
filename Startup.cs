using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Malaffi.Startup))]
namespace Malaffi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
