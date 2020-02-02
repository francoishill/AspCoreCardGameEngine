using System.Collections.Generic;
using System.Linq;
using AspCoreCardGameEngine.Domain.Models;
using AspCoreCardGameEngine.Domain.Models.Database;

namespace AspCoreCardGameEngine.Domain.Services.Responses
{
    public class PlayerPilesResponse
    {
        public List<CardSummary> DiscardPileCards { get; set; }
        public List<CardSummary> HandCards { get; set; }
        public List<CardSummary> FaceUpCards { get; set; }
        public List<CardSummary> FaceDownCards { get; set; }

        public PlayerPilesResponse(
            ICollection<Card> discardPileCards,
            ICollection<Card> handCards,
            ICollection<Card> faceUpCards,
            ICollection<Card> faceDownCards)
        {
            DiscardPileCards = discardPileCards.DefaultCardOrdering().Select(c => new CardSummary(c, true)).ToList();
            HandCards = handCards.DefaultCardOrdering().Select(c => new CardSummary(c, true)).ToList();
            FaceUpCards = faceUpCards.DefaultCardOrdering().Select(c => new CardSummary(c, true)).ToList();
            FaceDownCards = faceDownCards.DefaultCardOrdering().Select(c => new CardSummary(c, false)).ToList();
        }

        public class CardSummary
        {
            public int Id { get; set; }
            public bool IsVisible { get; set; }
            public CardSuit? Suit { get; set; }
            public int? Value { get; set; }

            public CardSummary(Card card, bool isVisible)
            {
                var value = card.Value;
                if (value == 1)
                {
                    // Ace
                    value = 14;
                }

                Id = card.Id;
                IsVisible = isVisible;
                Suit = isVisible ? card.Suit : (CardSuit?) null;
                Value = isVisible ? value : (int?) null;
            }
        }
    }
}