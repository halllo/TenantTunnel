﻿using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TenantTunnel.Auth
{
	public static class Policy
	{
		public const string TunnelRequest = nameof(TunnelRequest);
		public const string TunnelRespond = nameof(TunnelRespond);

		public static void Auth(this IServiceCollection services, IConfiguration config)
		{
			var validIssuers = File.ReadAllLines("./valid_issuers.txt").Select(i => i.Split('#')[0].Trim()).ToArray();

			services.AddAuthentication(options =>
			{
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.Authority = $"https://login.microsoftonline.com/{config["AzureAd:DirectoryId"]}";
				options.Audience = config["AzureAd:ApplicationId"];
				options.TokenValidationParameters.ValidIssuers = validIssuers;
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
					policy.AddRequirements(new HasAnyScopeRequirement("Tunnel.Respond", "Tunnel.Respond.Anonymous"));
				});
			});

			services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
			services.AddSingleton<IAuthorizationHandler, HasAnyScopeHandler>();
		}
	}


}
