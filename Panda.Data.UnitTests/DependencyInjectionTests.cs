using Microsoft.Extensions.DependencyInjection;
using Panda.Data.Pages;
using Xunit;

namespace Panda.Data.UnitTests;

public class DependencyInjectionTests
{
    [Fact]
    public void ContainerPassesVerification()
    {
        var services = new ServiceCollection();
        services.AddPandaDataServices();
        using var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions()
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });

        Assert.NotNull(serviceProvider);
    }

    [Fact]
    public void CanResolvePrimaryService()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.AddPandaDataServices().BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var pageReader = scope.ServiceProvider.GetRequiredService<IPageReader>();
        
        Assert.NotNull(pageReader);
    }
}