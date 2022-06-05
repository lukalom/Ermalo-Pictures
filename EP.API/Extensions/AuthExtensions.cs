using System.Text;
using EP.API.Security.Handlers;
using EP.API.Security.Requirements;
using EP.Infrastructure.Data;
using EP.Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace EP.API.Extensions
{
    internal static class AuthExtensions
    {
        public static IServiceCollection AddJwtConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            //Configure JWT
            var key = Encoding.ASCII.GetBytes(configuration["JwtConfig:Secret"]);
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true, // Validate jwt secret key
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = "https://localhost:44335/api/",
                ValidAudience = "https://localhost:44335/api/",
                //AudienceValidator = validation delegate
                //ValidIssuer = configuration["JwtSettings:Issuer"],
                //ValidAudience = configuration["JwtSettings:Audience"],
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // expire exactly at token expiration time
            };

            services.AddSingleton(tokenValidationParameters);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParameters;
            });

            return services;
        }

        public static IServiceCollection AddAuthorizationConfiguration(this IServiceCollection services)
        {
            //Identity Changed to ApplicationUser
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                    options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //Policy
            services.AddAuthorization(options =>
            {
                var policyBuilder = new AuthorizationPolicyBuilder();
                policyBuilder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                policyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = policyBuilder.Build();

                options.AddPolicy("OnlyNonBlockedUser", policy =>
                {
                    policy.Requirements.Add(new UserBlockedStatusRequirement(false));
                });

                options.AddPolicy("ShowPolicy", policy =>
                {
                    policy.Requirements.Add(new AccessShowManagementRequirement());
                });

                options.AddPolicy("RoleManagementPolicy",
                    policy => policy.RequireClaim("Role Management"));
            });

            //Password config
            services.Configure<IdentityOptions>(options =>
            { // Modifying Password input fields 
                options.Password.RequiredLength = 10;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
            });

            return services;
        }
    }
}
