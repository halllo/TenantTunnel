﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TenantTunnel.Auth
{
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
