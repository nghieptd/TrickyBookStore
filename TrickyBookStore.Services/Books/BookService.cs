﻿using System;
using System.Collections.Generic;
using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Books
{
    public class BookService : IBookService
    {
        public IList<Book> GetBooks(params long[] ids)
        {
            throw new NotImplementedException();
        }
    }
}
