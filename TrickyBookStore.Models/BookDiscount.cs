using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrickyBookStore.Models
{
    namespace BookDiscount
    {
        public static class OldBook
        {
            public static double Free = 0.1;
            public static double Paid = 0.95;
            public static double CategoryAddicted = 1;
            public static double Premium = 1;
        }
        public static class NewBook
        {
            public static double Free = 0;
            public static double Paid = 0.05;
            public static double CategoryAddicted = 0.15;
            public static double Premium = 0.15;
        }
    }
}
