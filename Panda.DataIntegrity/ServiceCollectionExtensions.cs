using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Panda.DataIntegrity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataIntegrityServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IChecksumProvider, Crc32CheckSumProvider>();
        return services;
    }
}