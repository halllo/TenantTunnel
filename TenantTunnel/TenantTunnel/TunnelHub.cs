using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TenantTunnel
{
	public class TunnelHub : Hub
	{
		private readonly ResponseCorrelations responseCorrelations;

		public TunnelHub(ResponseCorrelations responseCorrelations)
		{
			this.responseCorrelations = responseCorrelations;
		}

		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			await base.OnDisconnectedAsync(exception);
		}

		public async Task Respond(string correlationId, string result)
		{
			this.responseCorrelations.Yield(correlationId, result);
		}
	}
}
