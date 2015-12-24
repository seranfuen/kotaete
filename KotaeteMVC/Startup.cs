using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KotaeteMVC.Startup))]
namespace KotaeteMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
