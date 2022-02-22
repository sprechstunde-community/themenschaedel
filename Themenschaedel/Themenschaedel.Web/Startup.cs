using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazored.Toast;
using Themenschaedel.Shared.Models;
using Themenschaedel.Web.Services;
using Themenschaedel.Web.Services.Interfaces;

namespace Themenschaedel.Web
{
    public class Startup
    {
        //This will be used to allow offen to see collected opt-in data
        readonly string AllowSpecificOrigins = "_allowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("https://plausible.alyra.dev/themenschaedel.alyra.dev/",
                            "https://status.alyra.dev/",
                            "https://atleris.com/",
                            "https://alyra.dev/",
                            "https://api.schaedel.rocks/",
                            "https://localhost:44392/");
                    });
            });
            
            services.AddBlazoredSessionStorage();
            services.AddBlazoredToast();
            services.AddBlazoredLocalStorage();

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

            services.AddHttpClient<ISessionAPI, SessionAPI>(client =>
            {
                client.BaseAddress = new Uri(Configuration["APIURL"]);
            });
            
            services.AddScoped<Themenschaedel.Web.Services.Interfaces.IUserSession, Themenschaedel.Web.Services.UserSession>();
            services.AddScoped<IRefresher, EpisodeRefresher>();
            services.AddScoped<EpisodeCollectionRefresher>();

            services.AddHttpClient<IData, ApiData>(client =>
            {
                client.BaseAddress = new Uri(Configuration["APIURL"]);
            });
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(AllowSpecificOrigins);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
