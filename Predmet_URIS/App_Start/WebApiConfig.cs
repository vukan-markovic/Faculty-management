using Predmet_URIS.Handlers;
using Predmet_URIS.AuthFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Predmet_URIS
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
