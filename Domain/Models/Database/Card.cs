using System;

namespace AspCoreCardGameEngine.Domain.Models.Database
{
    public class Card : ICreatedDate, IUpdatedDate
    {
        public int Id { get; set; }

        public Pile Pile { get; set; }
        public CardSuitEnum Suit { get; set; }
        public int Value { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        private Card()
        {
        }

        public Card(Pile pile, CardSuitEnum suit, int value)
            : this()
        {
            Pile = pile;
            Suit = suit;
            Value = value;
        }

        public override string ToString()
        {
            return $"{GetValueString()} of {Suit.ToString()}";
        }

        private string GetValueString()
        {
            return Value switch
            {
                1 => "Ace",
                11 => "Jack",
                12 => "Queen",
                13 => "King",
                14 => "Joker",
                _ => Value.ToString()
            };
        }
    }
}