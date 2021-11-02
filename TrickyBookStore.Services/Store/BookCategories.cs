using System.Collections.Generic;
using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Store
{
    public static class BookCategories
    {
        public static readonly IEnumerable<BookCategory> Data = new List<BookCategory>
        {
            new BookCategory { Id = 1 },
            new BookCategory { Id = 2 },
            new BookCategory { Id = 3 },
            new BookCategory { Id = 4 },
        };
    }
}
