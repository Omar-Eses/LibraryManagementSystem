using LibraryManagementSystem.Data;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LibraryManagementSystem.CommonKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Library Management System API",
        Version = "v1"
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
            new string[] { }
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
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
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

builder.Services.AddLibraryManagementSystemModule();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LMSContext>();
    InsertPermissionsIfNotExists(context); // Call the method to insert permissions
}

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

app.Run();
return;

void InsertPermissionsIfNotExists(LMSContext context)
{
    if (context.Set<Permission>().Any()) return;
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