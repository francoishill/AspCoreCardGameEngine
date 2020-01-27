using System;
using System.Linq;
using System.Threading.Tasks;
using AspCoreCardGameEngine.Api.Persistence;
using AspCoreCardGameEngine.Api.ServiceImplementations.Shithead.Extensions;
using AspCoreCardGameEngine.Domain.Exceptions;
using AspCoreCardGameEngine.Domain.Models;
using AspCoreCardGameEngine.Domain.Models.Database;
using AspCoreCardGameEngine.Domain.Services;
using AspCoreCardGameEngine.Domain.Services.Requests;
using AspCoreCardGameEngine.Domain.Services.Responses;
using AspCoreCardGameEngine.Domain.Services.Structs;
using Microsoft.EntityFrameworkCore;

namespace AspCoreCardGameEngine.Api.ServiceImplementations.Shithead
{
    public class ShitheadGameEngine : IShitheadGameEngine
    {
        private readonly CardsDbContext _dbContext;
        private readonly IDeckFactory _deckFactory;

        public ShitheadGameEngine(
            CardsDbContext dbContext,
            IDeckFactory deckFactory)
        {
            _dbContext = dbContext;
            _deckFactory = deckFactory;
        }

        public async Task<CreateGameResponse> CreateGame(ShitheadGameConfig config, CreateShitheadGameRequest request)
        {
            await using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var game = new Game(ShitheadConstants.GAME_TYPE);
                await _dbContext.Games.AddAsync(game);
                await _dbContext.SaveChangesAsync();

                var deckCount = 1 + request.NumberPlayers / 5;
                var deckPile = new Pile(game, PileTypeEnum.Deck, ShitheadConstants.DECK_PILE_IDENTIFIER);
                for (var i = 0; i < deckCount; i++)
                {
                    _deckFactory.AddDeckCardsToPile(config, deckPile, new CreateDeckOptions
                    {
                        IncludeJokers = true,
                        Shuffled = true,
                    });
                }

                game.Piles.Add(deckPile);

                for (var i = 0; i < request.NumberPlayers; i++)
                {
                    var player = new Player(game);
                    game.Players.Add(player);
                    await _dbContext.SaveChangesAsync();

                    var faceDownPile = game.CreatePlayerPile(player, PlayerHandTypesEnum.FaceDown);
                    game.Piles.Add(faceDownPile);
                    var faceUpPile = game.CreatePlayerPile(player, PlayerHandTypesEnum.FaceUp);
                    game.Piles.Add(faceUpPile);
                    var handPile = game.CreatePlayerPile(player, PlayerHandTypesEnum.Hand);
                    game.Piles.Add(handPile);
                }

                var discardPile = new Pile(game, PileTypeEnum.Discard, ShitheadConstants.DISCARD_PILE_IDENTIFIER);
                game.Piles.Add(discardPile);

                var burnPile = new Pile(game, PileTypeEnum.Discard, ShitheadConstants.BURN_PILE_IDENTIFIER);
                game.Piles.Add(burnPile);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CreateGameResponse(game);
            }
        }

        private async Task<Game> GetGameOrThrow(Guid id)
        {
            var game = await _dbContext
                .GamesIncludingPlayersAndPilesAndCards
                .SingleOrDefaultAsync(g => g.Id == id && g.Type == ShitheadConstants.GAME_TYPE);

            if (game == null)
            {
                throw new DomainException(DomainErrorCode.EntityMissing, $"Shithead game with id {id} does not exist");
            }

            return game;
        }

        public async Task<JoinGameResponse> JoinGame(Guid id)
        {
            var game = await GetGameOrThrow(id);

            var firstAvailable = game.Players.FirstOrDefault(p => !p.Accepted);
            if (firstAvailable == null)
            {
                throw new DomainException(DomainErrorCode.Forbidden, "There are no available seats in the game");
            }

            firstAvailable.Accepted = true;
            await _dbContext.SaveChangesAsync();

            return new JoinGameResponse(firstAvailable.Id);
        }

        public async Task<DrawFromDeckResponse> DrawFromDeck(Guid id, Guid playerId)
        {
            var game = await GetGameOrThrow(id);

            var player = game.Players.SingleOrDefault(p => p.Id == playerId);
            if (player == null)
            {
                throw new DomainException(DomainErrorCode.EntityMissing, $"Player with {id} is not in the game");
            }

            var gameMutator = new ShitheadGameMutator(player);
            gameMutator.DrawFromDeck();
            await _dbContext.SaveChangesAsync();

            return new DrawFromDeckResponse();
        }
    }
}