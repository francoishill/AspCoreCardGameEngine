using System;
using System.Collections.Generic;

namespace AspCoreCardGameEngine.Api.Domain.Models.Database
{
    public class Card : ICreatedDate, IUpdatedDate
    {
        public int Id { get; set; }

        public Deck Deck { get; set; }
        public CardSuitEnum Suit { get; set; }
        public int Value { get; set; }

        public ICollection<CardImage> Images { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        private Card()
        {
            Images = new HashSet<CardImage>();
        }

        public Card(Deck deck, CardSuitEnum suit, int value)
            : this()
        {
            Deck = deck;
            Suit = suit;
            Value = value;
        }
    }
}