using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace TenantTunnel
{
	[ApiController]
	[Authorize(Policy.TunnelRequest)]
	public class TunnelController : ControllerBase
	{
		private readonly ResponseCorrelations responseCorrelations;
		private readonly IHubContext<TunnelHub> tunnelHub;

		public TunnelController(ResponseCorrelations responseCorrelations, IHubContext<TunnelHub> tunnelHub)
		{
			this.responseCorrelations = responseCorrelations;
			this.tunnelHub = tunnelHub;
		}

		[Route("api/tunnel/{endpoint}/{method}")]
		[HttpGet]
		public async Task<ActionResult> Get(string endpoint, string method)
		{
			var query = HttpContext.Request.Query;
			string argument = query[query.Keys.Single()].ToString();

			return await Request(endpoint, method, argument);
		}

		[Route("api/tunnel/{endpoint}/{method}")]
		[HttpPost]
		public async Task<ActionResult> Post(string endpoint, string method)
		{
			var reader = new StreamReader(HttpContext.Request.Body);
			var argument = reader.ReadToEnd();
			reader.Dispose();

			return await Request(endpoint, method, argument);
		}

		private async Task<ActionResult> Request(string endpoint, string method, string argument)
		{
			var response = this.responseCorrelations.Response(out string correlationId);
			await this.tunnelHub.Clients.All.SendAsync("Request", method, correlationId, argument);

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
