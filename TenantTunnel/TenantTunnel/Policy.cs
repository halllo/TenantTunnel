using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TenantTunnel
{
	public static class Policy
	{
		public const string TunnelRequest = nameof(TunnelRequest);
		public const string TunnelRespond = nameof(TunnelRespond);

		public static void Auth(this IServiceCollection services, IConfiguration config)
		{
			services.AddAuthentication(options =>
			{
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.Authority = $"https://login.microsoftonline.com/{config["AzureAd:DirectoryId"]}";
				options.Audience = config["AzureAd:ApplicationId"];
				options.TokenValidationParameters.ValidateIssuer = false;
			});

			services.AddAuthorization(options =>
			{
				options.AddPolicy(TunnelRequest, policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.AddRequirements(new HasScopeRequirement("Tunnel.Request"));
				});
				options.AddPolicy(TunnelRespond, policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.AddRequirements(new HasScopeRequirement("Tunnel.Respond"));
				});
			});

			services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
		}
	}







	public class HasScopeRequirement : IAuthorizationRequirement
	{
		public string Scope { get; }

		public HasScopeRequirement(string scope)
		{
			Scope = scope ?? throw new ArgumentNullException(nameof(scope));
		}
	}

	public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
		{
			// If user does not have the scope claim, get out of here
			if (!context.User.HasClaim(c => c.Type == "http://schemas.microsoft.com/identity/claims/scope"))
				return Task.CompletedTask;

			// Split the scopes string into an array
			var scopes = context.User.FindFirst(c => c.Type == "http://schemas.microsoft.com/identity/claims/scope").Value.Split(' ');

			// Succeed if the scope array contains the required scope
			if (scopes.Any(s => s == requirement.Scope))
				context.Succeed(requirement);

			return Task.CompletedTask;
		}
	}
}
