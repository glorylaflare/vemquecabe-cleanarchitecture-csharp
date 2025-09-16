using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using VemQueCabe.Api.Extensions;
using VemQueCabe.Api.Extensions.Policies.Handlers;
using VemQueCabe.Api.Middlewares;
using VemQueCabe.Infra.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services extensions to the container.
builder.Services.AddMapping();
builder.Services.AddUnitOfWork();
builder.Services.AddSwaggerServices();
builder.Services.AddApplicationServices();
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddAuthorizationServices();

builder.Services.AddSingleton<IAuthorizationHandler, SameUserHandler>();
builder.Services.AddScoped<IAuthorizationHandler, RideRequestOwnerHandler>();
builder.Services.AddScoped<IAuthorizationHandler, RideOwnerHandler>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        c =>
        {
            c.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (!app.Environment.IsDevelopment())       
{
    app.UseHsts();
}

if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("ENABLE_SWAGGER"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {

        c.InjectStylesheet("/swagger-ui/custom.css");
        c.InjectJavascript("/swagger-ui/custom.js");

        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VemQueCabe.Api API v1");
    });
}

app.UseMiddleware<ErrorHandleMiddleware>();

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();