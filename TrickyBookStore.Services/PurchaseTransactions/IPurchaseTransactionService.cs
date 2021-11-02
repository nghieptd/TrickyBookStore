using System;
using System.Collections.Generic;
using TrickyBookStore.Models;

namespace TrickyBookStore.Services.PurchaseTransactions
{
    // KeepIt
    public interface IPurchaseTransactionService
    {
        IList<PurchaseTransaction> GetPurchaseTransactions(long customerId, DateTimeOffset fromDate, DateTimeOffset toDate);
    }
}
