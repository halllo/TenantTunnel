using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TenantTunnel.Auth
{
	public class HasAnyScopeRequirement : IAuthorizationRequirement
	{
		public string[] Scopes { get; }

		public HasAnyScopeRequirement(params string[] scopes)
		{
			Scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
		}
	}

	public class HasAnyScopeHandler : AuthorizationHandler<HasAnyScopeRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasAnyScopeRequirement requirement)
		{
			var scopeClaims = context.User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/identity/claims/scope");
			var roleClaims = context.User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
			var permissionClaims = scopeClaims.Concat(roleClaims);

			// If user does not have the scope claim, get out of here
			if (!permissionClaims.Any())
				return Task.CompletedTask;

			// Split the scopes string into an array
			var scopes = permissionClaims.Select(c => c.Value).ToArray();

			// Succeed if the scope array contains one of the required scopes
			if (scopes.Intersect(requirement.Scopes).Any())
				context.Succeed(requirement);

			return Task.CompletedTask;
		}
	}
}
