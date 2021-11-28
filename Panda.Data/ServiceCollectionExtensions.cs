using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Panda.Data.Pages;
using Panda.Data.Pages.Free;
using Panda.Data.Pages.Root;

namespace Panda.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPandaDataServices(this IServiceCollection services)
    {
        services.AddRootPage();
        services.AddFreePage();
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
        services.AddSingleton<IPageDecoder<RootPage>, RootPageEncoder>();
        services.AddSingleton<IPageEncoder<RootPage>, RootPageEncoder>();
        services.AddSingleton<IUnknownPageDecoder, RootPageUnknownPageDecoder>();
        return services;
    }
}