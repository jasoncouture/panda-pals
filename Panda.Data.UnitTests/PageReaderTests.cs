using System;
using Microsoft.Extensions.DependencyInjection;
using Panda.Data.Pages;
using Panda.Data.Pages.Data;
using Panda.Data.Pages.Free;
using Panda.Data.Pages.Root;
using Xunit;

namespace Panda.Data.UnitTests;

public class PageReaderTests
{
    private readonly ServiceProvider _serviceProvider;

    public PageReaderTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddPandaDataServices();
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void UnknownPageDecoderCanHandleAllKnownPages()
    {
        var pageMemory = new[]
        {
            new Memory<byte>(new byte[512]),
            new Memory<byte>(new byte[512]),
            new Memory<byte>(new byte[512])
        };
        
        void SerializePage<T>(T page, Memory<byte> memory) where T : IPage<T>
        {
            _serviceProvider.GetRequiredService<IPageEncoder<T>>().EncodePage(page, memory);
        }

        var rootPage = new RootPage(1, 0, 2, 3ul, 4ul, Guid.NewGuid().ToByteArray());
        var freePage = new FreePage();
        var dataPage = new DataPage(1ul, 2, Guid.NewGuid().ToByteArray());

        SerializePage(rootPage, pageMemory[0]);
        SerializePage(freePage, pageMemory[1]);
        SerializePage(dataPage, pageMemory[2]);

        var pageReader = _serviceProvider.GetRequiredService<IPageReader>();

        var readRootPage = pageReader.ReadPage(pageMemory[0]);
        var readFreePage = pageReader.ReadPage(pageMemory[1]);
        var readDataPage = pageReader.ReadPage(pageMemory[2]);

        Assert.IsAssignableFrom<RootPage>(readRootPage);
        Assert.IsAssignableFrom<FreePage>(readFreePage);
        Assert.IsAssignableFrom<DataPage>(readDataPage);

    }
}