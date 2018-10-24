using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace TenantTunnel
{
	public class TenantTunnelLight
	{
		readonly HubConnection connection;
		readonly Dictionary<string, Tuple<Func<string, Task<string>>, Action>> handlers;
		private TenantTunnelLight(HubConnection connection)
		{
			this.connection = connection;
			this.handlers = new Dictionary<string, Tuple<Func<string, Task<string>>, Action>>();
		}

		public static async Task<TenantTunnelLight> Aim(string at, Func<Task<string>> accessToken = null)
		{
			var connection = new HubConnectionBuilder()
				.WithUrl(at, opt =>
				{
					if (accessToken != null)
					{
						opt.AccessTokenProvider = accessToken;
					}
				})
				.Build();
			var tenantTunnelLight = new TenantTunnelLight(connection);

			connection.On("Request", new[] { typeof(string), typeof(string), typeof(string) }, async message =>
			{
				var method = message[0].ToString();
				var correlationId = message[1].ToString();
				var body = message[2].ToString();

				if (tenantTunnelLight != null && tenantTunnelLight.handlers.ContainsKey(method))
				{
					var handler = tenantTunnelLight.handlers[method];
					var resultTask = handler.Item1(body);
					Respond(correlationId, resultTask, handler.Item2);
				}
			});

			async void Respond(string correlationId, Task<string> resultTask, Action responded)
			{
				var result = await resultTask;
				await Task.Delay(5000);
				await connection.InvokeAsync("Respond", correlationId, result);
				responded?.Invoke();
			}

			await connection.StartAsync();
			return tenantTunnelLight;
		}

		public void On(string method, Func<string, Task<string>> handler, Action responded = null)
		{
			this.handlers.Add(method, Tuple.Create(handler, responded));
		}

		public async Task Off()
		{
			await connection.StopAsync();
		}
	}
}
