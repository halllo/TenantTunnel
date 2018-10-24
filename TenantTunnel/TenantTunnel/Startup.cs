using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace TenantTunnel
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			services.AddSignalR();
			services.AddSingleton<ResponseCorrelations>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseDefaultFiles();
			app.UseStaticFiles();

			app.UseSignalR(routes =>
			{
				routes.MapHub<TunnelHub>("/hubs/tunnel");
			});
			app.UseMvc();
		}
	}
}
