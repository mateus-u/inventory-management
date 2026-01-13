using Application;
using Application.Common.Interfaces;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;
using WebAPI.Common.Filters;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseSerilog((hostContext, services, configuration) =>
{
    configuration.ReadFrom.Configuration(builder.Configuration);
});

builder.Services.AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>());

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Inventory Management API",
        Version = "v1",
        Description = "Inventory Management API"
    });
});

builder.Services.AddOpenApi();

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ICurrentUser, CurrentUser>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory Management API");
        options.RoutePrefix = string.Empty;
    });

    app.MapOpenApi();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();

        context.Database.Migrate();
        await ApplicationSeed.SeedSampleDataAsync(context);
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
