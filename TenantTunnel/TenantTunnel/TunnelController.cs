using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace TenantTunnel
{
	[ApiController]
	[Authorize(Auth.Policy.TunnelRequest)]
	public class TunnelController : ControllerBase
	{
		private readonly ResponseCorrelations responseCorrelations;
		private readonly Endpoints endpoints;

		public TunnelController(ResponseCorrelations responseCorrelations, Endpoints endpoints)
		{
			this.responseCorrelations = responseCorrelations;
			this.endpoints = endpoints;
		}

		[Route("api/tunnel/{endpoint}/{method}")]
		[HttpGet]
		public async Task<ActionResult> Get(string endpoint, string method, [FromQuery]string argument, [FromQuery]string isolation)
		{
			return await Request(endpoint, method, argument, isolation);
		}

		[Route("api/tunnel/{endpoint}/{method}")]
		[HttpPost]
		public async Task<ActionResult> Post(string endpoint, string method, [FromQuery]string isolation)
		{
			var reader = new StreamReader(HttpContext.Request.Body);
			var argument = reader.ReadToEnd();
			reader.Dispose();

			return await Request(endpoint, method, argument, isolation);
		}

		private async Task<ActionResult> Request(string endpoint, string method, string argument, string isolation)
		{
			var tenantId = this.HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
			var userId = isolation == "subject" ? this.HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value : null;

			var clients = await this.endpoints.Proxy(tenantId, userId, endpoint);

			var response = this.responseCorrelations.Response(out string correlationId);
			var request = clients.SendAsync("Request", method, correlationId, argument);

			await request;

			try
			{
				var result = await response;
				return Ok(result);
			}
			catch (TaskCanceledException e)
			{
				return StatusCode(408);
			}
		}
	}
}
