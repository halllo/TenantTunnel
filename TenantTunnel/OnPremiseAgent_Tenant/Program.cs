using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using TenantTunnel;

namespace OnPremiseAgent_Tenant
{
	class Program
	{
		static void Main(string[] args) => Run().Wait();
		static async Task Run()
		{
			try
			{
				var endpoint = "OnPremiseAgent";
				var method = "random1-100";

				Log("starting...", ConsoleColor.DarkGray);
				var login = new AsyncLazy<AuthenticationResult>(() => AcquireAccessToken());

				LightAim.Url = "https://localhost:44379/hubs/tunnel";
				var tunnelLight = await Light.For(
					endpoint: endpoint,
					accessToken: async () => (await login).AccessToken,
					closed: exception => Log(exception?.ToString() ?? "closed without exception", ConsoleColor.Red));

				tunnelLight.On(method, async message =>
				{
					Log($"{method}: {message}", ConsoleColor.Cyan);
					var response = new Random().Next(1, 100).ToString();
					Log("response: " + response);
					return response;
				}, responded: () => Log("sent"));

				Log("started", ConsoleColor.DarkGray);
				Log($"{endpoint}@@{(await login).TenantId ?? (await login).Authority.Replace("https://login.windows.net/", "").Replace("/", "")}.{method}", ConsoleColor.Magenta);

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
			var resource = "8afade94-3388-4016-a2d2-6ac272fcd54d";

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
