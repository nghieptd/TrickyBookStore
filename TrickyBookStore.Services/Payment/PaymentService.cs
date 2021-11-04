using System;
using System.Linq;
using System.Collections.Generic;
using TrickyBookStore.Models;
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

            Subscription highestSubscription = null;
            var categorialSubscriptions = new List<Subscription>();
            foreach (var subscription in customer.Subscriptions)
            {
                if (highestSubscription == null || highestSubscription.SubscriptionType < subscription.SubscriptionType)
                {
                    highestSubscription = subscription;
                }

                if (subscription.SubscriptionType == SubscriptionTypes.CategoryAddicted)
                {
                    categorialSubscriptions.Add(subscription);
                }
            }

            var subscriptionMonthlyPrice = 0.0;
            highestSubscription?.PriceDetails.TryGetValue("FixPrice", out subscriptionMonthlyPrice); 
            switch (highestSubscription?.SubscriptionType)
            {
                case SubscriptionTypes.Paid:
                    return subscriptionMonthlyPrice + GetTotalPriceAsPaid(transactions);
                case SubscriptionTypes.Premium:
                    return subscriptionMonthlyPrice + GetTotalPriceAsPremium(transactions);
                case SubscriptionTypes.CategoryAddicted:
                    var categorialIds = categorialSubscriptions.Where(sub => sub.BookCategoryId != null).Cast<int>().ToList();
                    return subscriptionMonthlyPrice + GetTotalPriceAsCategoryAddicted(transactions, categorialIds);
                default:
                    return GetTotalPriceAsFree(transactions);
            }
        }

        private double GetTotalPriceAsFree(IList<PurchaseTransaction> transactions)
        {
            return transactions.Aggregate(
                0.0,
                (total, transaction) => total + (transaction.Book.IsOld ? GetDiscountPrice(transaction.Book.Price, Models.BookDiscount.OldBook.Free) : transaction.Book.Price)
            );
        }
        private double GetTotalPriceAsPaid(IList<PurchaseTransaction> transactions)
        {
            double total = SubscriptionPrice.Paid;
            int newBookCount = 0;
            for (int i = 0; i < transactions.Count; i++)
            {
                var currentBook = transactions[i].Book;
                if (currentBook.IsOld)
                {
                    total += GetDiscountPrice(currentBook.Price, Models.BookDiscount.OldBook.Paid);
                }
                else if (newBookCount < 3)
                {
                    total += GetDiscountPrice(currentBook.Price, Models.BookDiscount.NewBook.Paid);
                    newBookCount++;
                }
                else
                {
                    total += currentBook.Price;
                }
            }

            return total;
        }
        private double GetTotalPriceAsPremium(IList<PurchaseTransaction> transactions)
        {
            double total = SubscriptionPrice.Premium;
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
                    total += GetDiscountPrice(currentBook.Price, Models.BookDiscount.NewBook.Premium);
                    newBookCount++;
                }
                else
                {
                    total += currentBook.Price;
                }
            }

            return total;
        }
        private double GetTotalPriceAsCategoryAddicted(IList<PurchaseTransaction> transactions, IList<int> categoryIds)
        {
            double total = SubscriptionPrice.CategoryAddicted;
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
                    total += GetDiscountPrice(currentBook.Price, Models.BookDiscount.NewBook.CategoryAddicted);
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
