using System;
using System.Linq;
using System.Collections.Generic;
using TrickyBookStore.Models;

namespace TrickyBookStore.Services.Subscriptions
{
    public class SubscriptionService : ISubscriptionService
    {
        public IList<Subscription> GetSubscriptions(params int[] ids)
        {
            var subscriptionsQuery = from subscription in Store.Subscriptions.Data
                                     where ids.Any(id => id == subscription.Id)
                                     select subscription;

            return subscriptionsQuery.ToList();
        }
    }
}
