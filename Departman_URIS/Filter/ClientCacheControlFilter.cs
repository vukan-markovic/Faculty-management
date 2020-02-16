using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Departman_URIS.Filters
{
    public enum ClientCacheControl
    {
        Public,
        Private,
        NoCache
    };

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ClientCacheControlFilterAttribute : ActionFilterAttribute
    {
        public ClientCacheControl CacheType;
        public double CacheSeconds;


        public ClientCacheControlFilterAttribute(double seconds = 60.0)
        {
            CacheType = ClientCacheControl.Private;
            CacheSeconds = seconds;
        }

        public ClientCacheControlFilterAttribute(ClientCacheControl cacheType, double seconds = 60.0)
        {
            CacheType = cacheType;
            CacheSeconds = seconds;
            if (cacheType == ClientCacheControl.NoCache)
                CacheSeconds = -1;
        }


        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);

            if (actionExecutedContext.Response == null)
                return;

            if (CacheType == ClientCacheControl.NoCache)
            {
                actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    NoStore = true
                };
                if (!actionExecutedContext.Response.Headers.Date.HasValue)
                    actionExecutedContext.Response.Headers.Date = DateTimeOffset.UtcNow;

                if (actionExecutedContext.Response.Content != null)
                    actionExecutedContext.Response.Content.Headers.Expires =
                        actionExecutedContext.Response.Headers.Date;
            }
            else
            {
                actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = (CacheType == ClientCacheControl.Public),
                    Private = (CacheType == ClientCacheControl.Private),
                    NoCache = false,
                    MaxAge = TimeSpan.FromSeconds(CacheSeconds)
                };
                if (!actionExecutedContext.Response.Headers.Date.HasValue)
                    actionExecutedContext.Response.Headers.Date = DateTimeOffset.UtcNow;

                if (actionExecutedContext.Response.Content != null)
                    actionExecutedContext.Response.Content.Headers.Expires =
                        actionExecutedContext.Response.Headers.Date;
            }
        }
    }
}