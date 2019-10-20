using System.Web.Mvc;
using System.Web.Routing;

namespace Triangles.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "TriangleFinder", action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}