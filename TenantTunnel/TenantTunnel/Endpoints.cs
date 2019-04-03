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



		public async Task Register(string tenantId, string userId, string endpoint, string connectionId, IGroupManager groups)
		{
			await groups.AddToGroupAsync(connectionId, GroupName(tenantId, userId, endpoint));
		}

		public async Task Unregister(string tenantId, string userId, string endpoint, string connectionId)
		{
			await Task.Yield();
		}

		public async Task<IClientProxy> Proxy(string tenantId, string userId, string endpoint)
		{
			return this.tunnelHub.Clients.Groups(GroupName(tenantId, userId, endpoint));
		}




		string GroupName(string tenantId, string userId, string endpoint) => $"{endpoint}@{userId}@{tenantId}";
	}
}
