using System.Collections.Generic;
using AspCoreCardGameEngine.Domain.Models;
using AspCoreCardGameEngine.Domain.Models.Database;
using AspCoreCardGameEngine.Domain.Services;
using AspCoreCardGameEngine.Domain.Services.Structs;

namespace AspCoreCardGameEngine.Api.ServiceImplementations
{
    public class DeckFactory : IDeckFactory
    {
        private readonly IShuffler _shuffler;

        public DeckFactory(
            IShuffler shuffler)
        {
            _shuffler = shuffler;
        }

        public void AddDeckCardsToPile(ShitheadGameConfig config, Pile pile, CreateDeckOptions options)
        {
            var cards = new List<Card>();
            for (var i = 1; i <= 13; i++)
            {
                cards.Add(new Card(pile, CardSuit.Clubs, i));
                cards.Add(new Card(pile, CardSuit.Diamonds, i));
                cards.Add(new Card(pile, CardSuit.Hearts, i));
                cards.Add(new Card(pile, CardSuit.Spades, i));
            }

            if (options.IncludeJokers)
            {
                for (var i = 0; i < 2; i++)
                {
                    cards.Add(new Card(pile, CardSuit.Joker, config.Joker));
                }
            }

            if (options.Shuffled)
            {
                _shuffler.Shuffle(cards);
            }

            foreach (var card in cards)
            {
                pile.Cards.Add(card);
            }
        }
    }
}