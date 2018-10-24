using System;
using System.Threading.Tasks;
using TenantTunnel;

namespace OnPremiseAgent
{
	class Program
	{
		static void Main(string[] args) => Run().Wait();
		static async Task Run()
		{
			Log("starting...", ConsoleColor.DarkGray);

			var tunnelLight = await TenantTunnelLight.Aim(
				at: "https://localhost:44379/hubs/tunnel",
				accessToken: () => Task.FromResult("ey..."));

			tunnelLight.On("request1", async message =>
			{
				Log($"request1: {message}", ConsoleColor.Cyan);
				var response = new Random().Next(1, 100).ToString();
				Log("Bitte Antwort eingeben: " + response);
				await Task.Delay(5000);
				return response;
			}, responded: () => Log("Gesendet."));

			Log("started", ConsoleColor.DarkGray);
			Console.ReadLine();

			await tunnelLight.Off();
		}

		static void Log(string message, ConsoleColor? color = null)
		{
			var currentColor = Console.ForegroundColor;
			if (color != null)
			{
				Console.ForegroundColor = color.Value;
			}
			Console.WriteLine(message);
			Console.ForegroundColor = currentColor;
		}
	}
}
