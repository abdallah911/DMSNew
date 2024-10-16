using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DMS_Authontication1
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        //    routes.MapRoute(
        //    name: "NetworkMedical",
        //    url: "NetworkMedical/{cardId}",
        //    defaults: new { controller = "NetworkMedical", action = "NetworkMedical", cardId = UrlParameter.Optional }
        //);

             routes.MapRoute(
            name: "NetworkMedical",
            url: "NetworkMedical/{action}/{cardId}",
            defaults: new { controller = "NetworkMedical", action = "NetworkMedical", cardId = UrlParameter.Optional }
        );          
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
