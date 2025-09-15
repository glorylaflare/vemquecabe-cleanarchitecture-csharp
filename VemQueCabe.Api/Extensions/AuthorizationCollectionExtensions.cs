using VemQueCabe.Api.Extensions.Policies.Requirements;

namespace VemQueCabe.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring authorization services in an application.
/// </summary>
public static class AuthorizationCollectionExtensions
{
    /// <summary>
    /// Configures authorization policies for the application.
    /// </summary>
    /// <param name="services">The service collection to add the authorization services to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("DriversOnly", policy =>
            {
                policy.RequireRole("Driver");
                policy.Requirements.Add(new SameUserRequirement());
            });
            options.AddPolicy("RideOwner", policy =>
            {
                policy.RequireRole("Driver");
                policy.Requirements.Add(new RideOwnerRequirement());
            });
    
            options.AddPolicy("PassengersOnly", policy =>
            {
                policy.RequireRole("Passenger");
                policy.Requirements.Add(new SameUserRequirement());
            });
            options.AddPolicy("RideRequestOwner", policy =>
            {
                policy.RequireRole("Passenger");
                policy.Requirements.Add(new RideRequestOwnerRequirement());
            });
        });
        
        return services;
    }
}