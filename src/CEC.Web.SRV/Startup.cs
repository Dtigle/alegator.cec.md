using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CEC.Web.SRV.Startup))]
namespace CEC.Web.SRV
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
