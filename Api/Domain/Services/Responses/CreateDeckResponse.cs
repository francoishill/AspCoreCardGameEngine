using System;
using System.Collections.Generic;
using System.Linq;
using AspCoreCardGameEngine.Api.Domain.Models;
using AspCoreCardGameEngine.Api.Domain.Models.Database;

namespace AspCoreCardGameEngine.Api.Domain.Services.Responses
{
    public class CreateDeckResponse
    {
        public Guid Id { get; }
        public List<DeckCard> Cards { get; }

        public CreateDeckResponse(Deck deck)
        {
            Id = deck.Id;
            Cards = deck.Cards.Select(c => new DeckCard(c)).ToList();
        }

        public class DeckCard
        {
            public CardSuitEnum Suit { get; set; }
            public int Value { get; set; }

            public DeckCard(Card card)
            {
                Suit = card.Suit;
                Value = card.Value;
            }
        }
    }
}