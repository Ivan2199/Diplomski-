using Newtonsoft.Json;
using System.Web.Http;
using System.Web.Http.Cors;

namespace GeoTagMap.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings =
                 new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}