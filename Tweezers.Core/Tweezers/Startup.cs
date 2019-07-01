using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Tweezers.Api.Middleware;
using Tweezers.MetadataManagement;

namespace Tweezers
{
    public class Startup
    {
        private readonly string corsConfig = "CorsConfig";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCors(options =>
            {
                options.AddPolicy(corsConfig,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200", "https://localhost:4200");
                        builder.WithMethods("GET", "POST", "PATCH", "PUT", "DELETE");
                        builder.WithHeaders("content-type", "accept");
                    });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Tweezers API",
                    Version = "v1",
                    Description = "Tweezers Project",
                    Contact = new Contact()
                    {
                        Name = "Tweezers",
                        Url = "https://github.com/tweezersCi/Tweezers.Core"
                    },
                });
            });

            TweezersMiddleware.AddIgnoreNullService(services);
        }

        // This method gets called on runtime. Use this method to configure the HTTP request pipeline.
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

            TweezersMiddleware.AddErrorHandler(app);
            TweezersMetadata.Init("tweezers-settings.json");

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseCors(corsConfig);
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
