using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspCoreCardGameEngine.Api.Domain.Models;
using AspCoreCardGameEngine.Api.Domain.Models.Database;
using AspCoreCardGameEngine.Api.Domain.Services;
using AspCoreCardGameEngine.Api.Domain.Services.Requests;
using AspCoreCardGameEngine.Api.Domain.Services.Responses;
using AspCoreCardGameEngine.Api.Persistence;
using Microsoft.Extensions.Logging;

namespace AspCoreCardGameEngine.Api.ServiceImplementations
{
    public class DeckService : IDeckService
    {
        private readonly ILogger<DeckService> _logger;
        private readonly CardsDbContext _dbContext;
        private readonly IShuffler _shuffler;

        public DeckService(
            ILogger<DeckService> logger,
            CardsDbContext dbContext,
            IShuffler shuffler)
        {
            _logger = logger;
            _dbContext = dbContext;
            _shuffler = shuffler;
        }

        public async Task<IEnumerable<DeckResponse>> CreateDecks(CreateDecksRequest request)
        {
            var decks = new List<Deck>();

            for (var i = 0; i < request.DeckCount; i++)
            {
                var deck = CreateDeck(new CreateDeckOptions
                {
                    IncludeJokers = request.IncludeJokers,
                    Shuffled = request.Shuffled,
                });
                decks.Add(deck);
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            await _dbContext.Decks.AddRangeAsync(decks);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogDebug("Decks created with Ids {0}", string.Join(", ", decks.Select(d => d.Id)));

            return decks.Select(d => new DeckResponse(d));
        }

        public struct CreateDeckOptions
        {
            public bool Shuffled;
            public bool IncludeJokers;
        }

        public Deck CreateDeck(CreateDeckOptions options)
        {
            var deck = new Deck();

            var cards = new List<Card>();
            for (var i = 1; i <= 13; i++)
            {
                cards.Add(new Card(deck, CardSuitEnum.Clubs, i));
                cards.Add(new Card(deck, CardSuitEnum.Diamonds, i));
                cards.Add(new Card(deck, CardSuitEnum.Hearts, i));
                cards.Add(new Card(deck, CardSuitEnum.Spades, i));
            }

            if (options.IncludeJokers)
            {
                for (var i = 0; i < 2; i++)
                {
                    cards.Add(new Card(deck, CardSuitEnum.Joker, 14));
                }
            }

            if (options.Shuffled)
            {
                _shuffler.Shuffle(cards);
            }

            foreach (var card in cards)
            {
                deck.Cards.Add(card);
            }

            return deck;
        }
    }
}