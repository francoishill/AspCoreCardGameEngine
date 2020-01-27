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
                cards.Add(new Card(pile, CardSuitEnum.Clubs, i));
                cards.Add(new Card(pile, CardSuitEnum.Diamonds, i));
                cards.Add(new Card(pile, CardSuitEnum.Hearts, i));
                cards.Add(new Card(pile, CardSuitEnum.Spades, i));
            }

            if (options.IncludeJokers)
            {
                for (var i = 0; i < 2; i++)
                {
                    cards.Add(new Card(pile, CardSuitEnum.Joker, config.Joker));
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