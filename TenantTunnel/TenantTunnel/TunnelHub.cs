using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TenantTunnel
{
	[Authorize(Auth.Policy.TunnelRespond)]
	public class TunnelHub : Hub
	{
		private readonly ResponseCorrelations responseCorrelations;
		private readonly Endpoints endpoints;

		public TunnelHub(ResponseCorrelations responseCorrelations, Endpoints endpoints)
		{
			this.responseCorrelations = responseCorrelations;
			this.endpoints = endpoints;
		}

		public override async Task OnConnectedAsync()
		{
			var httpContext = this.Context.GetHttpContext();
			var endpoint = httpContext.Request.Query["endpoint"].ToString();
			var isolation = httpContext.Request.Query["isolation"].ToString();
			var tenantId = this.Context.User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
			var userId = this.Context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

			await this.endpoints.Register(
				tenantId: tenantId,
				userId: isolation == "subject" ? userId : null,
				endpoint: endpoint,
				connectionId: this.Context.ConnectionId,
				groups: this.Groups);

			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			var httpContext = this.Context.GetHttpContext();
			var endpoint = httpContext.Request.Query["endpoint"].ToString();
			var isolation = httpContext.Request.Query["isolation"].ToString();
			var tenantId = this.Context.User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
			var userId = this.Context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

			await this.endpoints.Unregister(
				tenantId: tenantId,
				userId: isolation == "subject" ? userId : null,
				endpoint: endpoint,
				connectionId: this.Context.ConnectionId);

			await base.OnDisconnectedAsync(exception);
		}

		public async Task Respond(string correlationId, string result)
		{
			this.responseCorrelations.Yield(correlationId, result);
		}
	}
}
