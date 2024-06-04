﻿using Amdaris;
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
	[RoutePrefix("LiveLocalResults")]
	public class LiveLocalResultsController : ApiController
	{
		private readonly ILogger _logger;

		public LiveLocalResultsController(ILogger logger)
		{
			_logger = logger;
		}

		public HttpResponseMessage Get(Int64 pollId)
		{
			HttpContext.Current.AcceptWebSocketRequest(new LiveLocalResultsHandler(_logger));

			return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
		}

	}
}