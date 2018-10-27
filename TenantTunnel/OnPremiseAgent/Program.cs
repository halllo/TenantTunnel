using System;
using System.Configuration;
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
				var login = new AsyncLazy<AuthenticationResult>(() => AcquireAccessToken());

				TenantTunnelLightAim.Url = "https://localhost:44379/hubs/tunnel";
				var tunnelLight = await TenantTunnelLight.For(
					endpoint: "OnPremiseAgent",
					accessToken: async () => (await login).AccessToken,
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
				Log($"OnPremiseAgent@{(await login).TenantId}.random1-100", ConsoleColor.Magenta);

				Console.ReadLine();

				await tunnelLight.Off();
			}
			catch (Exception e)
			{
				Log(e.ToString(), ConsoleColor.Red);
				Console.ReadLine();
			}
		}


		private static async Task<AuthenticationResult> AcquireAccessToken()
		{
			var authority = ConfigurationManager.AppSettings["Authority"];
			var clientID = ConfigurationManager.AppSettings["ClientID"];
			var redirectUri = new Uri(ConfigurationManager.AppSettings["RedirectUri"]);
			var resource = "9ce9c8ac-4c70-42f6-8738-1183899d4960";

			Log("getting access token...");
			var authenticationContext = new AuthenticationContext(authority, false);
			var authenticationResult = await authenticationContext.AcquireTokenAsync(resource, clientID, redirectUri, new PlatformParameters(PromptBehavior.Auto));

			Log(new string(authenticationResult.AccessToken.Take(100).ToArray()) + "...", ConsoleColor.Green);

			return authenticationResult;
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
