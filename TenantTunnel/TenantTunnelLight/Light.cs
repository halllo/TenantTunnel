using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace TenantTunnel
{
	public class Light
	{
		readonly HubConnection connection;
		readonly Dictionary<string, Tuple<Func<string, Task<string>>, Action>> handlers;
		private Light(HubConnection connection)
		{
			this.connection = connection;
			this.handlers = new Dictionary<string, Tuple<Func<string, Task<string>>, Action>>();
		}

		public static async Task<Light> For(string endpoint, Func<Task<string>> accessToken = null, Action<Exception> closed = null)
		{
			var connection = new HubConnectionBuilder()
				.WithUrl(LightAim.Url + "?endpoint=" + endpoint, opt =>
				{
					if (accessToken != null)
					{
						opt.AccessTokenProvider = accessToken;
					}
				})
				.Build();
			var tenantTunnelLight = new Light(connection);

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
				await connection.InvokeAsync("Respond", correlationId, result);
				responded?.Invoke();
			}

			if (closed != null)
			{
				connection.Closed += async exception => closed?.Invoke(exception);
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

	public static class LightAim
	{
		public static string Url = "https://tenanttunnel.azurewebsites.net/hubs/tunnel";
	}
}
