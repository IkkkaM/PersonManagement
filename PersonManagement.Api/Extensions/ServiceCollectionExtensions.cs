namespace PersonDirectory.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.AddScoped<ValidationActionFilter>();

#pragma warning disable CS0618 
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationActionFilter>();
        })
        .AddFluentValidation(cfg =>
        {
            cfg.RegisterValidatorsFromAssemblyContaining<PersonCreateRequestValidator>();
            cfg.AutomaticValidationEnabled = true;
            cfg.DisableDataAnnotationsValidation = false;
        });
#pragma warning restore CS0618 

        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                    new CultureInfo("en-US"),
                    new CultureInfo("ka-GE")  
            };

            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;

            options.RequestCultureProviders.Clear();
            options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new()
            {
                Title = "Person Directory API",
                Version = "v1",
                Description = "API for managing person directory with Georgian language support"
            });
        });

        return services;
    }
}