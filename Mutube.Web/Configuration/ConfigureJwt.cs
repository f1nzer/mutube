using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Mutube.Database;
using Mutube.Database.Models.Identity;
using Mutube.Web.Options;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Mutube.Web.Configuration
{
    public static class ConfigureJwt
    {
        const string MIGRATIONS_ASSEMBLY = "Mutube.Database.Migrations";
        const string JWT_SETTINGS_SECTION = "JwtSettings";

        const string ENV_HOST = "DBHOST";
        const string ENV_HOST_DEFAULT = "localhost";
        const string ENV_PORT = "DBPORT";
        const string ENV_PORT_DEFAULT = "5432";
        const string ENV_USERNAME = "DBUSERNAME";
        const string ENV_USERNAME_DEFAULT = "postgres";
        const string ENV_PASSWORD = "DBPASSWORD";
        const string ENV_DEFAULT_PASSWORD = "20002000"; //TODO:
        const string ENV_DATABASE = "DBNAME";
        const string ENV_DEFAULT_DATABASE = "mutube";

        static IServiceCollection AddIdentityDatabaseInternal(this IServiceCollection service,
           IConfiguration configuration)
        {
            //TODO:
            var host = Environment.GetEnvironmentVariable(ENV_HOST);
            if (string.IsNullOrWhiteSpace(host))
            {
                host = ENV_HOST_DEFAULT;
            }
            var port = Environment.GetEnvironmentVariable(ENV_PORT);
            if (string.IsNullOrWhiteSpace(port))
            {
                port = ENV_PORT_DEFAULT;
            }
            var username = Environment.GetEnvironmentVariable(ENV_USERNAME);
            if (string.IsNullOrWhiteSpace(username))
            {
                username = ENV_USERNAME_DEFAULT;
            }

            var password = Environment.GetEnvironmentVariable(ENV_PASSWORD);
            if (string.IsNullOrWhiteSpace(password))
            {
                password = ENV_DEFAULT_PASSWORD;
            }

            var database = Environment.GetEnvironmentVariable(ENV_DATABASE);
            if (string.IsNullOrWhiteSpace(database))
            {
                database = ENV_DEFAULT_DATABASE;
            }

            var connectionString = $"Host={host};Port={port};Username={username};Password={password};Database={database}";

            service.AddDbContext<MutubeDbContext>(options =>
            {
                options.UseNpgsql(connectionString,
                    b => b.MigrationsAssembly(MIGRATIONS_ASSEMBLY));
                options.UseOpenIddict<long>();
            });

            return service;
        }

        static IServiceCollection AddIdentityInternal(this IServiceCollection service)
        {
            // Register the Identity services.
            service.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<MutubeDbContext>();

            service.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            return service;
        }

        static IServiceCollection AddOpenIddictInternal(
            this IServiceCollection service,
            IConfiguration configuration)
        {
            service.AddOpenIddict()
               .AddCore(options =>
               {
                   options.UseEntityFrameworkCore()
                         .UseDbContext<MutubeDbContext>()
                         .ReplaceDefaultEntities<long>();

               })
               .AddServer(options =>
               {
                   options.UseMvc();
                   options.EnableTokenEndpoint("/api/connect/token");
                   var issuer = new Uri(configuration.GetSection(JWT_SETTINGS_SECTION)["Authority"]);
                   options.SetIssuer(issuer);

                   options.AllowPasswordFlow()
                          .AllowRefreshTokenFlow();

                   options.AcceptAnonymousClients();

                   // develop case
                   options.DisableHttpsRequirement();

                   options.UseJsonWebTokens();

                   // develop case
                   options.AddEphemeralSigningKey();
               });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            return service;
        }

        static IServiceCollection AddBearerOptionsInternal(this IServiceCollection service, IConfiguration configuration)
        {
            service.Configure<JwtOptions>(configuration.GetSection(JWT_SETTINGS_SECTION));
            return service;
        }

        static IServiceCollection AddJwtAuthorizationInternal(
            this IServiceCollection service,
            IConfiguration configuration)
        {
            service.AddAuthorization(config =>
            {
                config.DefaultPolicy =
                    new AuthorizationPolicyBuilder(OpenIdConnectConstants.Schemes.Bearer)
                        .RequireClaim(OpenIdConnectConstants.Claims.Subject)
                        .Build();
            });

            return service;
        }

        static IServiceCollection AddJwtAuthenticationInternal(
            this IServiceCollection service,
            IConfiguration configuration)
        {
            service
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = OpenIdConnectConstants.Schemes.Bearer;
                    options.DefaultChallengeScheme = OpenIdConnectConstants.Schemes.Bearer;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;

                    options.Authority = configuration
                        .GetSection(JWT_SETTINGS_SECTION)["Authority"];
                    options.Audience = configuration
                        .GetSection(JWT_SETTINGS_SECTION)["Audience"];

                    // develop case
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = OpenIdConnectConstants.Claims.Subject,
                        RoleClaimType = OpenIdConnectConstants.Claims.Role
                    };
                });

            return service;
        }

        public static IServiceCollection ConfigureJwtIdentity(this IServiceCollection service, IConfiguration configuration)
        {
            service
                //.AddTickerBuilderInternal()
                .AddBearerOptionsInternal(configuration)
                .AddIdentityDatabaseInternal(configuration)
                .AddIdentityInternal()
                .AddOpenIddictInternal(configuration)
                .AddJwtAuthorizationInternal(configuration)
                .AddJwtAuthenticationInternal(configuration);

            return service;
        }
    }
}
