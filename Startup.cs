﻿using MediaCenterService.Applications;
using MediaCenterService.Power;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaCenterService
{
	public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
		
        public void ConfigureServices(IServiceCollection services)
        {
			services.AddSingleton<IApplicationManager, ApplicationManager>();
			services.AddSingleton<IPowerStateManager, PowerStateManager>();
            services.AddMvc();
        }
		
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
