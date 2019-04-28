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
        const string JWT_SETTINGS_SECTION = "JwtSettings";

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
                .AddIdentityInternal()
                .AddOpenIddictInternal(configuration)
                .AddJwtAuthorizationInternal(configuration)
                .AddJwtAuthenticationInternal(configuration);

            return service;
        }
    }
}
