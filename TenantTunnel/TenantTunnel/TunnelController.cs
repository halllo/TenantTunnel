using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace TenantTunnel
{
	[ApiController]
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
			var response = this.responseCorrelations.Response(out string correlationId);
			await this.tunnelHub.Clients.All.SendAsync("Request", method, correlationId, HttpContext.Request.QueryString.ToString());
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
