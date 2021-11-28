namespace Panda.Data.Pages;

public interface IPage<T> where T : IPage<T>
{
    PageType PageType { get; }
}

// The decoder does not need to know the page size.


// Data file format:
// Data file is broken up into pages of PageSize, (This allows us to use memory mapped views)
// The first page is assumed to be 512 bytes, this allows us to embed the file header in the root page (Which tells us the page size)
// Each page has a header, which is a 1 byte page type (see PageType enum), followed by PageSize-1 bytes.
// The data format of each page is determined by it's page type
// The root block points to each BTree root, and may have continuations (Marked as leaf nodes)
// Each BTree root can be considered it's own tree for parallelism.

// PageType.Free (0)
// This page can be allocated for another purpose. It's been marked free (DO NOT ASSUME IT IS ZEROED)
// This page contains PageSize-1 bytes, with no structure (Since this is a free page)


// PageType.RootContinuation (2)
// This page follows the format of 273+ from the root page, but is preceeded by:
// 1 - 8 - pointer to next continuation page, (0 if no more pages)
// 9+ repeats the branch root page pointer format from the root records.

// PageType.Branch (3)
// This page contains a list of BTree branches and leaves known.
// 1-4 - CRC32 of page data
// 4-12 - pointer to next branch page (0 if no more branches)
// Data - Formatted as follows (Repeats)
// 
// N to N+6 byte key, always interpreted as unsigned 64 bit number for BTree sort purposes.
// N+16 to N+24 data page pointer (0 if no record at this key)
// N+24 to N+32 page pointer for left branch/leaf (0 if no left branch, ignore page record)
// N+32 to N+40 page pointer for right branch/leaf (0 if no right branch, ignore page record)
// N+40 to N+44 page record for left branch/leaf (record number within the page)
// N+44 to N+48 page record for right branch/leaf (record number within the page)

// Every BTree node can also contain data, branch vs leaf is only determined by the data page pointer 

// PageType.Data (4)
// This page contains data, after a 4 byte CRC32 checksum of that data.

// PageType.FreePageIndex
// 1-4 CRC32 of the page data
// 4-12 pointer to next free page index page
// Page data is repeated as follows:
// Page Pointer to first free page
// 4 byte count of free pages following the previous page (0 if no longer free/can be re-used for another free page)

