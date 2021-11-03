using System;
using System.Linq;
using System.Collections.Generic;
using TrickyBookStore.Models;
using TrickyBookStore.Services.Books;

namespace TrickyBookStore.Services.PurchaseTransactions
{
    public class PurchaseTransactionService : IPurchaseTransactionService
    {
        IBookService BookService { get; }

        public PurchaseTransactionService(IBookService bookService)
        {
            BookService = bookService;
        }

        public IList<PurchaseTransaction> GetPurchaseTransactions(long customerId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            return Store.PurchaseTransactions.Data
                .Where(transaction => transaction.CustomerId == customerId && transaction.CreatedDate >= fromDate && transaction.CreatedDate <= toDate) 
                .Select(transaction => {
                    var books = BookService.GetBooks(transaction.BookId);
                    if (books.Count > 0)
                    {
                        transaction.Book = books[0];
                    }

                    return transaction;
                })
                .ToList();
        }
    }
}
