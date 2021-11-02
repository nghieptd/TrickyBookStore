using System;
using System.Linq;
using System.Collections.Generic;
using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Books
{
    public class BookService : IBookService
    {
        public IList<Book> GetBooks(params long[] ids)
        {
            var booksQuery = from book in Store.Books.Data
                             where ids.Any(id => id == book.Id)
                             select book;

            return booksQuery.ToList();
        }
    }
}
