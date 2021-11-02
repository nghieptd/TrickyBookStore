using System;
using Microsoft.Extensions.DependencyInjection;
using TrickyBookStore.Services.Payment;
using TrickyBookStore.Services.Customers;
using TrickyBookStore.Services.PurchaseTransactions;
using TrickyBookStore.Services.Subscriptions;
using TrickyBookStore.Services.Books;

namespace TrickyBookStore.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton<ISubscriptionService, SubscriptionService>()
                .AddSingleton<IBookService, BookService>()
                .AddSingleton<ICustomerService, CustomerService>()
                .AddSingleton<IPurchaseTransactionService, PurchaseTransactionService>()
                .AddSingleton<IPaymentService, PaymentService>()
                .BuildServiceProvider();

            var payment = serviceCollection
                .GetService<IPaymentService>();
            var fromDate = new DateTimeOffset(2021, 7, 1, 0, 0, 0, new TimeSpan());
            var toDate = new DateTimeOffset(2021, 7, 1, 0, 0, 0, new TimeSpan());
            payment.GetPaymentAmount(1, fromDate, toDate);

            Console.WriteLine("Hello World!");
        }
    }
}
