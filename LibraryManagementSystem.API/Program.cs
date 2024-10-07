using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LibraryManagementSystem.Application;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Helpers;
using LibraryManagementSystem.Domain.Models;
using LibraryManagementSystem.Infrastructure;
using LibraryManagementSystem.Infrastructure.Data;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);


// configure serilog with open telemtry
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/lms-service.txt", rollingInterval: RollingInterval.Day
    )
    .WriteTo.OpenTelemetry(x =>
    {
        x.Endpoint = "http://localhost:5341/ingest/otlp/v1/logs";
        x.Protocol = OtlpProtocol.HttpProtobuf;
        x.Headers = new Dictionary<string, string>
        {
            ["X-Seq-ApiKey"] = "NVtf3vW9kz0q90JQPik9"
        };
        x.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = "LMS1",
            ["deployment.environment"] = builder.Environment.EnvironmentName
        };
    })
    .CreateLogger();
builder.Services.AddSerilog();
// configure serilog logger
/*
 * Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.File(
        "logs/lms-service.log", rollingInterval: RollingInterval.Hour
    ).CreateLogger();
builder.Services.AddSerilog();
*/
/*
// Configure open telemetry logger
builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(x =>
{
    x.IncludeScopes = true;
    x.IncludeFormattedMessage = true;
    x.SetResourceBuilder(ResourceBuilder.CreateEmpty().AddService("LMS service").AddAttributes(new Dictionary<string, object>()
    {
        ["deployment.environment"] = builder.Environment.EnvironmentName
    }));
    x.AddOtlpExporter(a =>
    {
        a.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
        a.Protocol = OtlpExportProtocol.HttpProtobuf;
        a.Headers = "X-Seq-ApiKey=HcbBqyR6LStgg9bH83PC";
    });
});
*/
builder.Services.AddControllers();
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Library Management System API",
        Version = "v1.1"
    });

    // Add JWT bearer token configuration
    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT Bearer token in the format: Bearer {your token here}",
    };

    var securityRequirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "bearer"
                }
            },
            []
        }
    };

    options.AddSecurityDefinition("bearer", securityScheme);
    options.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["JwtSettings:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Key is not configured.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(PermissionTypes.CanBorrow, policy => policy.RequireClaim("permission", PermissionTypes.CanBorrow))
    .AddPolicy(PermissionTypes.CanReturn, policy => policy.RequireClaim("permission", PermissionTypes.CanReturn))
    .AddPolicy(PermissionTypes.CanAddBook, policy => policy.RequireClaim("permission", PermissionTypes.CanAddBook))
    .AddPolicy(PermissionTypes.CanDeleteBook,
        policy => policy.RequireClaim("permission", PermissionTypes.CanDeleteBook))
    .AddPolicy(PermissionTypes.CanEditBook, policy => policy.RequireClaim("permission", PermissionTypes.CanEditBook))
    .AddPolicy(PermissionTypes.CanGetBook, policy => policy.RequireClaim("permission", PermissionTypes.CanGetBook));

builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureLmsApplication(builder.Configuration);
builder.Services.ConfigureLmsInfrastructure(builder.Configuration);
var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.UseMigrationsEndPoint();
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.Logger.LogInformation("Adding Routes");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    InsertPermissionsIfNotExists(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Logger.LogInformation("Starting App");

app.Run();
return;

void InsertPermissionsIfNotExists(ApplicationDbContext context)
{
    if (context.Set<Permission>().Any())
    {
        return;
    }

    context.Set<Permission>().AddRange(
        new Permission { Id = 1, PermissionName = PermissionTypes.CanBorrow },
        new Permission { Id = 2, PermissionName = PermissionTypes.CanReturn },
        new Permission { Id = 3, PermissionName = PermissionTypes.CanAddBook },
        new Permission { Id = 4, PermissionName = PermissionTypes.CanGetBook },
        new Permission { Id = 5, PermissionName = PermissionTypes.CanDeleteBook },
        new Permission { Id = 6, PermissionName = PermissionTypes.CanEditBook }
    );
    context.SaveChanges();
}