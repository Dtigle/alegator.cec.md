using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Amdaris;
using CEC.Web.Results.Api.Infrastructure;
using Microsoft.Web.WebSockets;

namespace CEC.Web.Results.Api.Controllers
{
    [HandleAndLogError]
    [RoutePrefix("LiveElectionTurnout")]
    public class LiveElectionTurnoutController : ApiController
    {
        private readonly ILogger _logger;

        public LiveElectionTurnoutController(ILogger logger)
        {
            _logger = logger;
        }

        public HttpResponseMessage Get()
        {
            HttpContext.Current.AcceptWebSocketRequest(new LiveElectionTurnoutHandler(_logger));

            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }
    }
}
