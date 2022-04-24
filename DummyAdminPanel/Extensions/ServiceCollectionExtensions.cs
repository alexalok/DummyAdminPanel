using Blazored.LocalStorage;
using DummyAdminPanel;
using DummyAdminPanel.Authentication;
using DummyAdminPanel.Connectors.DummyApi;
using DummyAdminPanel.Connectors.DummyApi.API;
using DummyAdminPanel.Connectors.DummyApi.LocalStorage;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStaticAuthenticationService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<IAuthenticationService, StaticAuthenticationService>();
        return services;
    }

    public static IServiceCollection AddDummyApiConnector(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.AddOptions<DummyApiSettings>()
            .Bind(configurationSection)
            .Validate(s => s.AppId != null!)
            .Validate(s => Uri.TryCreate(s.BaseUrl, UriKind.Absolute, out _))
            .Validate(s => s.PageSize is >= 5 and <= 50) // https://dummyapi.io/docs
            .Validate(s => s.CacheLifetimeSeconds >= 0); 

        services.AddHttpClient<IDummyApiConnector, DummyApiConnector>((srv, http) =>
        {
            var settings = srv.GetRequiredService<IOptions<DummyApiSettings>>().Value;
            http.BaseAddress = new Uri(settings.BaseUrl);
            http.DefaultRequestHeaders.Add("app-id", settings.AppId);
        });
        return services;
    }

    public static IServiceCollection AddDummyApiLocalStorageConnector(this IServiceCollection services)
    {
        services.AddBlazoredLocalStorage();
        services.AddScoped<ICachingDummyApiConnector, DummyApiLocalStorageConnector>();
        services.AddScoped<IDummyApiCacher, DummyApiLocalStorageConnector>();
        return services;
    }
}