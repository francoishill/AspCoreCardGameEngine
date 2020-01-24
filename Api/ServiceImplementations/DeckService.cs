using System.Threading.Tasks;
using AspCoreCardGameEngine.Api.Domain.Models;
using AspCoreCardGameEngine.Api.Domain.Models.Database;
using AspCoreCardGameEngine.Api.Domain.Services;
using AspCoreCardGameEngine.Api.Domain.Services.Requests;
using AspCoreCardGameEngine.Api.Domain.Services.Responses;
using AspCoreCardGameEngine.Api.Persistence;

namespace AspCoreCardGameEngine.Api.ServiceImplementations
{
    public class DeckService : IDeckService
    {
        private readonly CardsDbContext _dbContext;

        public DeckService(CardsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CreateDeckResponse> CreateDeck(CreateDeckRequest request)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            var deck = new Deck();
            await _dbContext.Decks.AddAsync(deck);
            await _dbContext.SaveChangesAsync();

            var cards = new[]
            {
                new Card(deck, CardSuitEnum.Clubs, 1),
                new Card(deck, CardSuitEnum.Clubs, 2),
                new Card(deck, CardSuitEnum.Diamonds, 1),
                new Card(deck, CardSuitEnum.Diamonds, 2),
                new Card(deck, CardSuitEnum.Hearts, 1),
                new Card(deck, CardSuitEnum.Hearts, 2),
                new Card(deck, CardSuitEnum.Spades, 1),
                new Card(deck, CardSuitEnum.Spades, 2),
            };
            await _dbContext.Cards.AddRangeAsync(cards);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return new CreateDeckResponse(deck);
        }
    }
}