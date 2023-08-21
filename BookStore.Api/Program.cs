using System.Data;
using System.Text.Json.Serialization;
using BookStore.Api.Middlewares;
using BookStore.Api.ServiceInjection;
using BookStore.Application.Mappers;
using BookStore.CrossCutting.Security;
using BookStore.CrossCutting.Security.Entities;
using BookStore.Infra.Data;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

var jwtConfiguration = builder.Configuration.GetSection("Jwt").Get<JwtConfiguration>();
builder.Services.AddSingleton(jwtConfiguration);

builder.Services.AddSecurity(jwtConfiguration);

builder.Services
    .AddAppServices()
    .AddRepositories()
    .AddDomain()
    .AddDomainEvents();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Host.UseSerilog();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    // Require Bearer token authentication
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
            new string[] { }
        }
    });
});

builder.Services.AddLogging(builder => builder.AddSerilog());

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidDataException("Connection string is invalid");

builder.Services.AddScoped<IDbConnection>(_ => new DbConnection(connectionString));

var mapper = AutoMapperConfig.Setup();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionMiddleware>();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

// Use Serilog for logging
app.UseSerilogRequestLogging(); // Logs HTTP request information

app.Run();