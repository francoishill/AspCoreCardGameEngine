using System;
using System.Collections.Generic;
using System.Linq;
using AspCoreCardGameEngine.Domain.Models.Database;

namespace AspCoreCardGameEngine.Domain.Services
{
    public static class ShitheadHelpers
    {
        public static IEnumerable<Card> DefaultCardOrdering(this IEnumerable<Card> cards)
        {
            // TODO: Ordering by .Updated and ThenBy .Id is not super robust
            return cards.OrderBy(c => c.Updated ?? DateTime.MinValue).ThenBy(c => c.Id);
        }
    }
}