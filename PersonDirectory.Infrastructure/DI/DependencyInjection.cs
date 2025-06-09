using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonDirectory.Domain.Repositories;
using PersonDirectory.Domain.Repositories.UOW;
using PersonDirectory.Infrastructure.Persistence;
using PersonDirectory.Infrastructure.Repositories;
using PersonDirectory.Infrastructure.Repositories.UOW;

namespace PersonDirectory.Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Context
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IPhoneNumberRepository, PhoneNumberRepository>();
        services.AddScoped<IPersonConnectionRepository, PersonConnectionRepository>();

        return services;
    }
}