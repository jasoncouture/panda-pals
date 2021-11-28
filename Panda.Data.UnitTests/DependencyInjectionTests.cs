using Microsoft.Extensions.DependencyInjection;
using Panda.Data.Pages;
using Xunit;
using Xunit.Abstractions;

namespace Panda.Data.UnitTests;

public class DependencyInjectionTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public DependencyInjectionTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

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

    [Fact]
    public void DumpContainerServices()
    {
        var services = new ServiceCollection();
        services.AddPandaDataServices();

        foreach (var service in services)
        {
            _testOutputHelper.WriteLine(service.ToString());
        }
    }
}