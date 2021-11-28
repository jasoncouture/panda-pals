using System;
using Microsoft.Extensions.DependencyInjection;
using Panda.Data.Pages;
using Panda.Data.Pages.Data;
using Panda.Data.Pages.Free;
using Panda.Data.Pages.Root;
using Panda.Data.PageStore;
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
        var pageStore = new MemoryPageStore();
        var pageReader = _serviceProvider.GetRequiredService<IPageReader>();

        void SerializePage<T>(T page, ulong pageNumber) where T : IPage<T>
        {
            using var pageBlock = pageStore.OpenPage(pageNumber);
            _serviceProvider.GetRequiredService<IPageEncoder<T>>().EncodePage(page, pageBlock.Memory);
            pageBlock.MakeDurable();
        }

        IUnknownPage? ReadPage(ulong pageNumber)
        {
            using var pageBlock = pageStore.OpenReadOnlyPage(pageNumber);
            return pageReader.ReadPage(pageBlock.ReadOnlyMemory);
        }

        var rootPage = new RootPage(1, 0, 2, 3ul, 4ul, Guid.NewGuid().ToByteArray());
        var freePage = new FreePage();
        var dataPage = new DataPage(1ul, 2, Guid.NewGuid().ToByteArray());


        SerializePage(rootPage, 0);
        SerializePage(freePage, 1);
        SerializePage(dataPage, 2);



        var readRootPage = ReadPage(0);
        var readFreePage = ReadPage(1);
        var readDataPage = ReadPage(2);

        Assert.IsAssignableFrom<RootPage>(readRootPage);
        Assert.IsAssignableFrom<FreePage>(readFreePage);
        Assert.IsAssignableFrom<DataPage>(readDataPage);

    }
}