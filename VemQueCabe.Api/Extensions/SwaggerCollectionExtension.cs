using System.Reflection;
using Microsoft.OpenApi.Models;

namespace VemQueCabe.Api.Extensions;

/// <summary>
/// Classe de extensão para configuração do Swagger na aplicação.
/// </summary>
public static class SwaggerCollectionExtension
{
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "VemQueCabe API",
                Version = "v1",
                Description = "API do aplicativo de carona mais legal do mundo!",
                Contact = new OpenApiContact
                {
                    Name = "Marcelo Gonçalves",
                    Email = "marcelojunior_@outlook.com.br",
                    Url = new Uri("https://github.com/glorylaflare")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = $@"Autorização JWT usando o Bearer.
                    </br>Coloque 'Bearer'[espaço] e então o seu token em seguida.
                    </br>Exemplo: \'Bearer 12345abcdef\'</br>",
                });
        });

        return services;
    }
}
