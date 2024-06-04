using Amdaris;
using CEC.Web.Results.Api.Infrastructure;
using Microsoft.Web.WebSockets;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CEC.Web.Results.Api.Controllers
{

	[HandleAndLogError]
	[RoutePrefix("LiveResults")]
	public class LiveResultsController : ApiController
	{
		private readonly ILogger _logger;

		public LiveResultsController(ILogger logger)
		{
			_logger = logger;
		}

		public HttpResponseMessage Get(Int64 pollId)
		{
			HttpContext.Current.AcceptWebSocketRequest(new LiveResultsHandler(_logger));

			return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
		}

	}
}