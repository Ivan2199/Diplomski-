using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using GeoTagMap.Service.Common;
using System;
using System.Web.Http;

[assembly: OwinStartup(typeof(GeoTagMap.WebApi.Startup))]

namespace GeoTagMap.WebApi
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            ConfigureOAuth(app);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            var userService = (IUserService)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUserService));

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/user/login"),
                Provider = new ApplicationAuthProvider(userService),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(3),
                AllowInsecureHttp = false
            };

            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}