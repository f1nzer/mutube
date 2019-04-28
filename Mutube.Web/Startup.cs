using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mutube.Database;
using Mutube.Web.Configuration;
using Mutube.Web.Hubs;

namespace Mutube.Web
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
            SetupDatabase(services);
            
            services.ConfigureJwtIdentity(Configuration);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        private static void SetupDatabase(IServiceCollection services)
        {
            var host = Environment.GetEnvironmentVariable("DBHOST") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("DBPORT") ?? "5432";
            var username = Environment.GetEnvironmentVariable("DBUSERNAME") ?? "postgres";
            var password = Environment.GetEnvironmentVariable("DBPASSWORD") ?? "20002000";
            var database = Environment.GetEnvironmentVariable("DBNAME") ?? "mutube";
            var connectionString = $"Host={host};Port={port};Username={username};Password={password};Database={database}";

            services.AddDbContext<MutubeDbContext>(options =>
            {
                options.UseNpgsql(connectionString,
                    builder => builder.MigrationsAssembly("Mutube.Database.Migrations"));
                options.UseOpenIddict<long>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Mutube.Database.MutubeDbContext db)
        {
            if (env.IsDevelopment())
            {
                db.Database.Migrate();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSignalR(routes =>
            {
                routes.MapHub<RoomsHub>("/hub/rooms");
            });
        }
    }
}