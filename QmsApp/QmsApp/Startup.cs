using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QmsApp.Startup))]
namespace QmsApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
