using System;

namespace AspCoreCardGameEngine.Api.Domain.Models.Database
{
    public class Card : ICreatedDate, IUpdatedDate
    {
        public int Id { get; set; }

        public Deck Deck { get; set; }
        public CardSuitEnum Suit { get; set; }
        public int Value { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        private Card()
        {
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