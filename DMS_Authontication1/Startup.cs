using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DMS_Authontication1.Startup))]
namespace DMS_Authontication1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();

        }
    }
}
