using aex.devtest.application.Middleware;
using aex.devtest.Controllers;
using aex.devtest.domain.CSV.Interfaces;
using aex.devtest.domain.CSV.Services;
using aex.devtest.infrastucture.Context;
using aex.devtest.infrastucture.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.InputFormatters.Add(new ByteArrayInputFormatter()))
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.Converters.Add(new StringEnumConverter()));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AEX.DevTest.API", Version = "v1" });
    c.EnableAnnotations();
});

builder.Services.AddSwaggerGenNewtonsoftSupport();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services & Repositories
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

var app = builder.Build();

// Ef
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetService<ILogger<ApplicationDbContext>>();
    logger.LogDebug("Applying database migrations...");

    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

    // Migrate db
    context.Database.Migrate();
}

// Exception handling
app.UseExceptionResponseHandler();
app.UseExceptionLogHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
