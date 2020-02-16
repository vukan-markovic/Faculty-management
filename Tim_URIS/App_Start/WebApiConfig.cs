using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Tim_URIS.AuthFilters;
using Tim_URIS.Handlers;

namespace Tim_URIS
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            config.SuppressHostPrincipal();

            //messages
            config.MessageHandlers.Add(new PipelineTimerHandler());

            //auth
            config.Filters.Add(new BasicAuthFilterAttribute());
            config.Filters.Add(new TokenAuthFilter());

            config.Filters.Add(new AuthorizeAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();
        }
    }
}
