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

            while (true)
            {
                var userId = ReadUserId();
                var month = ReadMonth();
                var year = ReadYear();

                var fromDate = new DateTimeOffset(new DateTime(year, month, 1));
                var toDate = fromDate.AddMonths(1).AddDays(-1);
                var totalPayment = payment.GetPaymentAmount(userId, fromDate, toDate);
                Console.WriteLine($"=> Total payment: {totalPayment} USD");
                 
                Console.WriteLine("Press ESC to stop OR press any key to continue.\n");
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
        }
        static int ReadUserId()
        {
            var id = -1;
            bool valid = false;
            do
            {
                Console.Write("User ID: ");
                var stringId = Console.ReadLine();
                try
                {
                    id = int.Parse(stringId);
                    valid = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid ID input. Try again.");
                }
            } while (!valid);

            return id;
        }
        static int ReadMonth()
        {
            var month = 0;
            bool valid = true;
            do
            {
                Console.Write("Month: ");
                var stringMonth = Console.ReadLine();

                try
                {
                    month = int.Parse(stringMonth);
                    valid = true;
                } catch (FormatException)
                {
                    Console.WriteLine("Invalid month input (must be between 1 and 12). Try again.");
                }
            } while (!valid);

            return month;
        }
        static int ReadYear()
        {
            var year = 0;
            bool valid = false;

            do
            {
                Console.Write("Year: ");
                var stringYear = Console.ReadLine();

                try
                {
                    year = int.Parse(stringYear);
                    valid = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid year input (must be between 1 and 9999). Try again.");
                }
            } while (!valid);

            return year;
        }
    }
}
