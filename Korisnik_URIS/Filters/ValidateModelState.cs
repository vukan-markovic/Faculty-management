using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Korisnik_URIS.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public bool BodyRequired { get; set; } = false;

        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState);
            }
            else if (BodyRequired)
            {
                foreach (var b in actionContext.ActionDescriptor.ActionBinding.ParameterBindings)
                {
                    if (b.WillReadBody)
                    {
                        if (!actionContext.ActionArguments.ContainsKey(b.Descriptor.ParameterName)
                            || actionContext.ActionArguments[b.Descriptor.ParameterName] == null)
                        {
                            actionContext.Response = actionContext.Request.CreateErrorResponse(
                                HttpStatusCode.BadRequest, b.Descriptor.ParameterName + " is required.");
                        }
                        break;
                    }
                }
            }

            await base.OnActionExecutingAsync(actionContext, cancellationToken);
        }
    }
}