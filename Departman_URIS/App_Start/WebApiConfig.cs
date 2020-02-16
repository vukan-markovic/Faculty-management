using DepartmanURIS.AuthFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Departman_URIS
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            config.SuppressHostPrincipal();


            config.Filters.Add(new BasicAuthFilterAttribute());
            config.Filters.Add(new TokenAuthFilter());

            config.Filters.Add(new AuthorizeAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();
        }
    }
}
