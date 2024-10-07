using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace LibraryManagementSystem.Infrastructure;

public static class LmsInfrastructureModule
{
    public static void ConfigureLmsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("ConnectionStrings");
        Console.WriteLine(connectionString);

        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseNpgsql(
                connectionString["LibraryManagementSystemContext"] ??
                throw new InvalidOperationException("Connection string 'LibraryManagementSystemContext' not found.")
            )
        );
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }
}