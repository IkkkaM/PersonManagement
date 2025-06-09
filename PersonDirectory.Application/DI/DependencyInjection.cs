namespace PersonDirectory.Application.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // Application Services
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IFileService, FileService>();

        // FluentValidation
        //services.AddValidatorsFromAssembly(typeof(PersonCreateRequestValidator).Assembly);

        return services;
    }
}