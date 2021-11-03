using System;
using System.Linq;
using System.Collections.Generic;
using TrickyBookStore.Services.Books;
using TrickyBookStore.Services.Customers;
using TrickyBookStore.Services.PurchaseTransactions;

namespace TrickyBookStore.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        ICustomerService CustomerService { get; }
        IPurchaseTransactionService PurchaseTransactionService { get; }

        public PaymentService(ICustomerService customerService, 
            IPurchaseTransactionService purchaseTransactionService)
        {
            CustomerService = customerService;
            PurchaseTransactionService = purchaseTransactionService;
        }

        public double GetPaymentAmount(long customerId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            var transactions = PurchaseTransactionService.GetPurchaseTransactions(customerId, fromDate, toDate);
            if (transactions.Count < 1)
            {
                return 0;
            }
            var customer = CustomerService.GetCustomerById(customerId);

            Models.SubscriptionTypes highestSubscriptionType = Models.SubscriptionTypes.Free;
            var categorialSubscriptions = new List<Models.Subscription>();
            foreach (var subscription in customer.Subscriptions)
            {
                if (highestSubscriptionType < subscription.SubscriptionType)
                {
                    highestSubscriptionType = subscription.SubscriptionType;
                }

                if (subscription.SubscriptionType == Models.SubscriptionTypes.CategoryAddicted)
                {
                    categorialSubscriptions.Add(subscription);
                }
            }

            switch (highestSubscriptionType)
            {
                case Models.SubscriptionTypes.Paid:
                    return GetTotalPriceAsPaid(transactions);
                case Models.SubscriptionTypes.Premium:
                    return GetTotalPriceAsPremium(transactions);
                case Models.SubscriptionTypes.CategoryAddicted:
                    var categorialIds = categorialSubscriptions.Where(sub => sub.BookCategoryId != null).Cast<int>().ToList();
                    return GetTotalPriceAsCategoryAddicted(transactions, categorialIds);
                default:
                    return GetTotalPriceAsFree(transactions);
            }
        }

        private double GetTotalPriceAsFree(IList<Models.PurchaseTransaction> transactions)
        {
            return transactions.Aggregate(
                (double)Models.SubscriptionPrice.Free,
                (total, transaction) => total + (transaction.Book.IsOld ? GetDiscountPrice(transaction.Book.Price, 0.1) : transaction.Book.Price)
            );
        }
        private double GetTotalPriceAsPaid(IList<Models.PurchaseTransaction> transactions)
        {
            double total = Models.SubscriptionPrice.Paid;
            int newBookCount = 0;
            for (int i = 0; i < transactions.Count; i++)
            {
                var currentBook = transactions[i].Book;
                if (currentBook.IsOld)
                {
                    total += GetDiscountPrice(currentBook.Price, 0.95);
                }
                else if (newBookCount < 3)
                {
                    total += GetDiscountPrice(currentBook.Price, 0.05);
                    newBookCount++;
                }
                else
                {
                    total += currentBook.Price;
                }
            }

            return total;
        }
        private double GetTotalPriceAsPremium(IList<Models.PurchaseTransaction> transactions)
        {
            double total = Models.SubscriptionPrice.Premium;
            int newBookCount = 0;
            for (int i = 0; i < transactions.Count; i++)
            {
                var currentBook = transactions[i].Book;
                if (currentBook.IsOld)
                {
                    continue;
                }
                else if (newBookCount < 3)
                {
                    total += GetDiscountPrice(currentBook.Price, 0.15);
                    newBookCount++;
                }
                else
                {
                    total += currentBook.Price;
                }
            }

            return total;
        }
        private double GetTotalPriceAsCategoryAddicted(IList<Models.PurchaseTransaction> transactions, IList<int> categoryIds)
        {
            double total = Models.SubscriptionPrice.CategoryAddicted;
            int newBookCount = 0;
            for (int i = 0; i < transactions.Count; i++)
            {
                var currentBook = transactions[i].Book;
                if (!categoryIds.Any(id=> id == currentBook.CategoryId))
                {
                    total += currentBook.Price;
                    continue;
                }

                if (currentBook.IsOld)
                {
                    continue;
                }
                else if (newBookCount < 3)
                {
                    total += GetDiscountPrice(currentBook.Price, 0.15);
                    newBookCount++;
                }
                else
                {
                    total += currentBook.Price;
                }
            }

            return total;
        }
        private double GetDiscountPrice(double basePrice, double discountRatio) => basePrice * (1 - discountRatio);
    }
}
