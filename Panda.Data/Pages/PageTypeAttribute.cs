namespace Panda.Data.Pages;

[AttributeUsage(AttributeTargets.Class)]
public class PageTypeAttribute : Attribute
{
    public PageType PageType { get; }

    public PageTypeAttribute(PageType pageType)
    {
        PageType = pageType;
    }
}