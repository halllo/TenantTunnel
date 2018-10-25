using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using TenantTunnel;

namespace OnPremiseAgent
{
	class Program
	{
		static void Main(string[] args) => Run().Wait();
		static async Task Run()
		{
			try
			{
				Log("starting...", ConsoleColor.DarkGray);

				TenantTunnelLightAim.Url = "https://localhost:44379/hubs/tunnel";
				var tunnelLight = await TenantTunnelLight.For(
					endpoint: "OnPremiseAgent",
					accessToken: async () => await AcquireAccessToken(),
					closed: exception => Log(exception.ToString(), ConsoleColor.Red));

				tunnelLight.On("random1-100", async message =>
				{
					Log($"random1-100: {message}", ConsoleColor.Cyan);
					var response = new Random().Next(1, 100).ToString();
					Log("Bitte Antwort eingeben: " + response);
					//await Task.Delay(1000);
					return response;
				}, responded: () => Log("Gesendet."));

				Log("started", ConsoleColor.DarkGray);
				Console.ReadLine();

				await tunnelLight.Off();
			}
			catch (Exception e)
			{
				Log(e.ToString(), ConsoleColor.Red);
				Console.ReadLine();
			}
		}


		private static async Task<string> AcquireAccessToken()
		{
			var authority = "https://login.windows.net/74a15b6f-bbcc-4e59-b372-6ec940a12bf3";
			var clientID = "31ee10a2-6d71-43e6-b3ea-5ab2b0b89d0a";
			var rediretUri = new Uri("https://tenanttunnellight_manuels_pc");
			var resource = "9ce9c8ac-4c70-42f6-8738-1183899d4960";

			Log("getting access token...");
			var authenticationContext = new AuthenticationContext(authority, false);
			var authenticationResult = await authenticationContext.AcquireTokenAsync(resource, clientID, rediretUri, new PlatformParameters(PromptBehavior.Auto));
			Log(new string(authenticationResult.AccessToken.Take(100).ToArray()) + "...", ConsoleColor.Green);

			return authenticationResult.AccessToken;
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
