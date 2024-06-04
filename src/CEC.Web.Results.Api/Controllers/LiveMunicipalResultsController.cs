using System;
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
	[RoutePrefix("LiveMunicipalResults")]
	public class LiveMunicipalResultsController : ApiController
	{
		private readonly ILogger _logger;

		public LiveMunicipalResultsController(ILogger logger)
		{
			_logger = logger;
		}

		public HttpResponseMessage Get(Int64 pollId)
		{
			HttpContext.Current.AcceptWebSocketRequest(new LiveMunicipalResultsHandler(_logger));

			return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
		}

	}
}