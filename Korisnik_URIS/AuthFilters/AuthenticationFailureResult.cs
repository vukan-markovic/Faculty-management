using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Korisnik_URIS.AuthFilters
{
    public class AuthenticationFailureResult : IHttpActionResult
    {
        public string ReasonPhrase { get; private set; }
        public HttpRequestMessage Request { get; private set; }
        public AuthenticationFailureResult(HttpRequestMessage request)
        {
            ReasonPhrase = null;
            Request = request;
        }
        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }
        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;

            if (!string.IsNullOrEmpty(ReasonPhrase))
                response.ReasonPhrase = ReasonPhrase;
            else
                response.ReasonPhrase = Enum.GetName(typeof(HttpStatusCode), HttpStatusCode.Unauthorized);

            return response;
        }
    }
}