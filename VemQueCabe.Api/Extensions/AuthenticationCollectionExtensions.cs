using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace VemQueCabe.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring authentication services in an application.
/// </summary>
public static class AuthenticationCollectionExtensions
{
    /// <summary>
    /// Configures JWT Bearer authentication services.
    /// </summary>
    /// <param name="services">The service collection to add the authentication services to.</param>
    /// <param name="configuration">The application configuration containing JWT settings.</param>
    /// <returns></returns>
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var secret = Encoding.UTF8.GetBytes(configuration["JWT:Secret"]);

        services.AddAuthentication(options => 
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JWT:Issuer"],
                ValidAudience = configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(secret),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "text/plain";
 
                    var result = new
                    {
                        error = "Authentication is required to access this resource."
                    };
                    return context.Response.WriteAsJsonAsync(result.error);
                }
            };
        });

        return services;
    }
}
