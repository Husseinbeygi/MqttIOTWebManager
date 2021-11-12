using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MqttService;
using MqttService.Configuration;
using System.Reflection;

namespace MqttWebAppTest
{
    public class Startup
    {
        private readonly AssemblyName serviceName = Assembly.GetExecutingAssembly().GetName();

        private readonly MqttConfiguration mqttServiceConfiguration = new();
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton(this.mqttServiceConfiguration);


            Configuration.GetSection(this.serviceName.Name).Bind(this.mqttServiceConfiguration);



            services.AddRazorPages();
            services.AddSingleton(_ => new Bootstrapper(this.mqttServiceConfiguration,this.serviceName.Name));
            services.AddSingleton<IHostedService>(p => p.GetRequiredService<Bootstrapper>());

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");


                app.UseHsts(); // see https://aka.ms/aspnetcore-hsts.
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

            });

        }

    }
}
