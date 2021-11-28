namespace Panda.Data.Pages;

public enum PageType : byte
{
    Free = 0,
    Root = 1,
    RootContinuation = 2,
    Branch = 3,
    Data = 4,
    FreePageIndex = 5,
    Unknown = 0xFF // Corrupt, Free, or uninitialized.
}