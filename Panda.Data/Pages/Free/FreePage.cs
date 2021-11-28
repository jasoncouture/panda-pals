using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panda.Data.Pages.Free;

[PageType(PageType.Free)]
public record FreePage(bool Zero = false) : IPage<FreePage>
{
    public PageType PageType => PageType.Free;
}