using Microsoft.Extensions.DependencyInjection;
using Panda.Data.Pages;
using Panda.Data.Pages.Data;
using Panda.Data.Pages.Free;
using Panda.Data.Pages.Root;
using Panda.DataIntegrity;

namespace Panda.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPandaDataServices(this IServiceCollection services)
    {
        services.AddRootPage();
        services.AddFreePage();
        services.AddDataPage();
        services.AddDataIntegrityServices();
        services.AddSingleton<IPageReader, PageReader>();
        services.AddSingleton<IPageTypeIdentifier, PageTypeIdentifier>();
        return services;
    }

    internal static IServiceCollection AddFreePage(this IServiceCollection services)
    {
        services.AddSingleton<IPageDecoder<FreePage>, FreePageDecoder>();
        services.AddSingleton<IPageEncoder<FreePage>, FreePageEncoder>();
        services.AddSingleton<IUnknownPageDecoder, FreePageUnknownPageDecoder>();
        return services;
    }

    internal static IServiceCollection AddRootPage(this IServiceCollection services)
    {
        services.AddSingleton<IPageDecoder<RootPage>, RootPageDecoder>();
        services.AddSingleton<IPageEncoder<RootPage>, RootPageEncoder>();
        services.AddSingleton<IUnknownPageDecoder, RootPageUnknownPageDecoder>();
        return services;
    }

    internal static IServiceCollection AddDataPage(this IServiceCollection services)
    {
        services.AddSingleton<IPageDecoder<DataPage>, DataPageDecoder>();
        services.AddSingleton<IPageEncoder<DataPage>, DataPageEncoder>();
        services.AddSingleton<IUnknownPageDecoder, DataPageUnknownPageDecoder>();
        return services;
    }
}