using System;
using System.Collections.Generic;

namespace AspCoreCardGameEngine.Domain.Models.Database
{
    public class Deck : ICreatedDate, IUpdatedDate
    {
        public Guid Id { get; set; }

        public ICollection<Card> Cards { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public Deck()
        {
            Cards = new HashSet<Card>();
        }
    }
}