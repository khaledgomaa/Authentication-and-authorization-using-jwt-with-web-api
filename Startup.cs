using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(jwt.Startup))]
namespace jwt
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            Configurationjwt(app);
        }
    }
}
