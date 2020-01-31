using System;
using System.Collections.Generic;
using System.Linq;

namespace AspCoreCardGameEngine.Domain.Models.Database
{
    public class Pile : ICreatedDate, IUpdatedDate
    {
        public int Id { get; set; }
        public Guid GameId { get; set; }
        public Game Game { get; set; }
        public PileType Type { get; set; }
        public string Identifier { get; set; }

        public ICollection<Card> Cards { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        private Pile()
        {
            Cards = new HashSet<Card>();
        }

        public Pile(Game game, PileType type, string identifier)
            : this()
        {
            Game = game;
            Type = type;
            Identifier = identifier;
        }

        public override string ToString()
        {
            return $"Id={Id}, GameId={GameId}, Type={Type}, Identifier={Identifier}";
        }

        public bool IsEmpty()
        {
            return Cards.Count == 0;
        }

        public bool ContainsAllCards(IEnumerable<Card> cards, out IEnumerable<Card> cardsMissingFromPile)
        {
            var cardsList = cards.ToList();
            var pileCardsList = Cards.ToList();

            var sharedCards = pileCardsList.Intersect(cardsList).ToList();
            if (sharedCards.Count == cardsList.Count)
            {
                cardsMissingFromPile = null;
                return true;
            }

            cardsMissingFromPile = cardsList.Except(sharedCards);
            return false;
        }
    }
}