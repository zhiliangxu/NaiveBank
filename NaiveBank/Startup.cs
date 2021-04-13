using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            if (!string.IsNullOrWhiteSpace(Configuration.GetValue<string>("Cors:AllowedOrigin")))
            {
                app.UseCors(builder =>
                {
                    builder.WithOrigins(Configuration.GetValue<string>("Cors:AllowedOrigin"));
                    if (!string.IsNullOrWhiteSpace(Configuration.GetValue<string>("Cors:AllowedMethods")))
                    {
                        builder.WithMethods(Configuration.GetValue<string>("Cors:AllowedMethods").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    }

                    if (!string.IsNullOrWhiteSpace(Configuration.GetValue<string>("Cors:AllowedHeaders")))
                    {
                        builder.WithHeaders(Configuration.GetValue<string>("Cors:AllowedHeaders").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    }

                    if (bool.TryParse(Configuration.GetValue<string>("Cors:AllowCredentials"), out bool allowCredentials) && allowCredentials)
                    {
                        builder.AllowCredentials();
                    }
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
