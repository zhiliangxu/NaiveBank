using Microsoft.AspNetCore.Builder;

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NaiveBank
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            if (!string.IsNullOrWhiteSpace(Configuration.GetValue<string>("AllowedOrigin")))
            {
                app.UseCors(builder =>
                {
                    builder.WithOrigins(Configuration.GetValue<string>("AllowedOrigin"));
                    if (!string.IsNullOrWhiteSpace(Configuration.GetValue<string>("AllowedMethods")))
                    {
                        builder.WithMethods(Configuration.GetValue<string>("AllowedMethods").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    }

                    if (!string.IsNullOrWhiteSpace(Configuration.GetValue<string>("AllowedHeaders")))
                    {
                        builder.WithHeaders(Configuration.GetValue<string>("AllowedHeaders").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    }

                    if (bool.TryParse(Configuration.GetValue<string>("AllowCredentials"), out bool allowCredentials) && allowCredentials)
                    {
                        builder.AllowCredentials();
                    }
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
