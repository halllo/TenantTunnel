using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TenantTunnel
{
	[Authorize(Policy.TunnelRespond)]
	public class TunnelHub : Hub
	{
		private readonly ResponseCorrelations responseCorrelations;

		public TunnelHub(ResponseCorrelations responseCorrelations)
		{
			this.responseCorrelations = responseCorrelations;
		}

		public override async Task OnConnectedAsync()
		{
			var httpContext = this.Context.GetHttpContext();
			var endpoint = httpContext.Request.Query["endpoint"].ToString();
			var tenantId = this.Context.User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;

			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			var httpContext = this.Context.GetHttpContext();
			var endpoint = httpContext.Request.Query["endpoint"].ToString();
			var tenantId = this.Context.User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;

			await base.OnDisconnectedAsync(exception);
		}

		public async Task Respond(string correlationId, string result)
		{
			this.responseCorrelations.Yield(correlationId, result);
		}
	}
}
