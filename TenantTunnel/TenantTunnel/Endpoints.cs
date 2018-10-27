using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TenantTunnel
{
	public class Endpoints
	{
		private readonly IHubContext<TunnelHub> tunnelHub;

		public Endpoints(IHubContext<TunnelHub> tunnelHub)
		{
			this.tunnelHub = tunnelHub;
		}

		public async Task Register(string tenantId, string endpoint, string connectionId, IGroupManager groups)
		{
			await groups.AddToGroupAsync(connectionId, GroupName(tenantId, endpoint));
		}

		public async Task Unregister(string tenantId, string endpoint, string connectionId)
		{
			await Task.Yield();
		}

		public async Task<IClientProxy> Proxy(string tenantId, string endpoint)
		{
			return this.tunnelHub.Clients.Groups(GroupName(tenantId, endpoint));
		}

		string GroupName(string tenantId, string endpoint) => $"{endpoint}@{tenantId}";
	}
}
