namespace Panda.Data.Pages;

public interface IPageTypeIdentifier
{
    PageType DeterminePageType(ReadOnlyMemory<byte> memory);
}