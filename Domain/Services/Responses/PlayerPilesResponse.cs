using System.Collections.Generic;
using System.Linq;
using AspCoreCardGameEngine.Domain.Models;
using AspCoreCardGameEngine.Domain.Models.Database;

namespace AspCoreCardGameEngine.Domain.Services.Responses
{
    public class PlayerPilesResponse
    {
        public List<CardSummary> HandCards { get; set; }
        public List<CardSummary> FaceUpCards { get; set; }
        public List<CardSummary> FaceDownCards { get; set; }

        public PlayerPilesResponse(ICollection<Card> handCards, ICollection<Card> faceUpCards, ICollection<Card> faceDownCards)
        {
            HandCards = handCards.Select(c => new CardSummary(c, true)).ToList();
            FaceUpCards = faceUpCards.Select(c => new CardSummary(c, true)).ToList();
            FaceDownCards = faceDownCards.Select(c => new CardSummary(c, false)).ToList();
        }

        public class CardSummary
        {
            public int Id { get; set; }
            public bool IsVisible { get; set; }
            public CardSuit? Suit { get; set; }
            public int? Value { get; set; }

            public CardSummary(Card card, bool isVisible)
            {
                Id = card.Id;
                IsVisible = isVisible;
                Suit = isVisible ? card.Suit : (CardSuit?) null;
                Value = isVisible ? card.Value : (int?) null;
            }
        }
    }
}