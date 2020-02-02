using System;
using System.Collections.Generic;
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
        private readonly IShitheadPileLogic _shitheadPileLogic;
        private readonly IRealtimeService _realtimeService;

        public ShitheadGameEngine(
            CardsDbContext dbContext,
            IDeckFactory deckFactory,
            IShitheadPileLogic shitheadPileLogic,
            IRealtimeService realtimeService)
        {
            _dbContext = dbContext;
            _deckFactory = deckFactory;
            _shitheadPileLogic = shitheadPileLogic;
            _realtimeService = realtimeService;
        }

        public async Task<CreateGameResponse> CreateGame(ShitheadGameConfig config, CreateShitheadGameRequest request)
        {
            await using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                var game = new Game(ShitheadConstants.GAME_TYPE, ShitheadConstants.GameModes.NORMAL);
                await _dbContext.Games.AddAsync(game);
                await _dbContext.SaveChangesAsync();

                var deckCount = 1 + request.NumberPlayers / 5;
                var deckPile = new Pile(game, PileType.Deck, ShitheadConstants.PileIdentifiers.DECK);
                for (var i = 0; i < deckCount; i++)
                {
                    _deckFactory.AddDeckCardsToPile(config, deckPile, new CreateDeckOptions
                    {
                        IncludeJokers = true,
                        Shuffled = true,
                    });
                }

                game.Piles.Add(deckPile);

                var playerFaceDownPiles = new List<Pile>();
                var playerFaceUpPiles = new List<Pile>();
                var playerHandPiles = new List<Pile>();
                for (var i = 0; i < request.NumberPlayers; i++)
                {
                    var player = new Player(game);
                    game.Players.Add(player);
                    await _dbContext.SaveChangesAsync();

                    var faceDownPile = game.CreatePlayerPile(player, PlayerHandType.FaceDown);
                    game.Piles.Add(faceDownPile);
                    playerFaceDownPiles.Add(faceDownPile);

                    var faceUpPile = game.CreatePlayerPile(player, PlayerHandType.FaceUp);
                    game.Piles.Add(faceUpPile);
                    playerFaceUpPiles.Add(faceUpPile);

                    var handPile = game.CreatePlayerPile(player, PlayerHandType.Hand);
                    game.Piles.Add(handPile);
                    playerHandPiles.Add(handPile);
                }

                for (var i = 1; i <= config.FaceDownCount; i++)
                {
                    foreach (var faceDownPile in playerFaceDownPiles)
                    {
                        var firstDeckCard = deckPile.Cards.First();
                        deckPile.Cards.Remove(firstDeckCard);
                        faceDownPile.Cards.Add(firstDeckCard);
                    }
                }

                for (var i = 1; i <= config.FaceUpCount; i++)
                {
                    foreach (var faceUpPile in playerFaceUpPiles)
                    {
                        var firstDeckCard = deckPile.Cards.First();
                        deckPile.Cards.Remove(firstDeckCard);
                        faceUpPile.Cards.Add(firstDeckCard);
                    }
                }

                for (var i = 1; i <= config.HandCount; i++)
                {
                    foreach (var handPile in playerHandPiles)
                    {
                        var firstDeckCard = deckPile.Cards.First();
                        deckPile.Cards.Remove(firstDeckCard);
                        handPile.Cards.Add(firstDeckCard);
                    }
                }

                var discardPile = new Pile(game, PileType.Discard, ShitheadConstants.PileIdentifiers.DISCARD);
                game.Piles.Add(discardPile);

                var burnPile = new Pile(game, PileType.Discard, ShitheadConstants.PileIdentifiers.BURN);
                game.Piles.Add(burnPile);

                game.State = game.Players.First().Id.ToString();

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

        public async Task<JoinGameResponse> JoinGame(Guid gameId)
        {
            var game = await GetGameOrThrow(gameId);

            var firstAvailable = game.Players.FirstOrDefault(p => !p.Accepted);
            if (firstAvailable == null)
            {
                throw new DomainException(DomainErrorCode.Forbidden, "There are no available seats in the game");
            }

            firstAvailable.Accepted = true;
            await _dbContext.SaveChangesAsync();

            return new JoinGameResponse(firstAvailable.Id);
        }

        private static void ValidateIsPlayerTurn(Game game, Player player)
        {
            if (game == null) throw new ArgumentNullException(nameof(game));
            if (player == null) throw new ArgumentNullException(nameof(player));
            if (game.State != player.Id.ToString())
            {
                throw new DomainException(DomainErrorCode.BadRequest, string.Format(Resources.It_is_another_player_s_turn__not_player__0_, player.Id));
            }
        }

        public async Task DrawFromDeck(Guid gameId, Guid playerId)
        {
            var game = await GetGameOrThrow(gameId);

            var player = game.Players.SingleOrDefault(p => p.Id == playerId);
            if (player == null)
            {
                throw new DomainException(DomainErrorCode.EntityMissing, $"Player with {gameId} is not in the game");
            }

            ValidateIsPlayerTurn(game, player);

            _shitheadPileLogic.DrawFromDeck(player);
            await _dbContext.SaveChangesAsync();
        }

        public async Task PlayCards(
            ShitheadGameConfig config,
            Guid gameId,
            Guid playerId,
            PlayShitheadCardsRequest request)
        {
            var game = await GetGameOrThrow(gameId);

            var player = game.Players.SingleOrDefault(p => p.Id == playerId);
            if (player == null)
            {
                throw new DomainException(DomainErrorCode.EntityMissing, $"Player with {gameId} is not in the game");
            }

            ValidateIsPlayerTurn(game, player);

            var playerPile = player.GetFirstNonEmptyPile();
            var matchingCards = request.CardIds.Select(id => new
                {
                    CardId = id,
                    Card = playerPile.Cards.SingleOrDefault(c => c.Id == id),
                })
                .ToList();

            var missingCards = matchingCards.Where(m => m.Card == null).ToList();
            if (missingCards.Any())
            {
                var joinedIds = string.Join(", ", missingCards.Select(m => m.CardId));
                throw new DomainException(DomainErrorCode.BadRequest, $"Cards (ids {joinedIds}) do not exist or cannot be found in the pile of player {player.Id} (game {game.Id})");
            }

            var cardsToPlay = matchingCards.Select(m => m.Card).ToList();

            _shitheadPileLogic.PlayToDeck(config, player, cardsToPlay);
            await _dbContext.SaveChangesAsync();

            await _realtimeService.OnGameMove(game.Id.ToString(), player.Id.ToString());
        }

        public async Task TakeDiscardPile(ShitheadGameConfig config, Guid gameId, Guid playerId)
        {
            var game = await GetGameOrThrow(gameId);

            var player = game.Players.SingleOrDefault(p => p.Id == playerId);
            if (player == null)
            {
                throw new DomainException(DomainErrorCode.EntityMissing, $"Player with {gameId} is not in the game");
            }

            ValidateIsPlayerTurn(game, player);

            var playerHandPile = player.GetHandPile();

            _shitheadPileLogic.PickUpDiscardPile(config, player, playerHandPile);
            await _dbContext.SaveChangesAsync();

            await _realtimeService.OnGameMove(game.Id.ToString(), player.Id.ToString());
        }

        public async Task<PlayerPilesResponse> GetPlayerPiles(Guid gameId, Guid playerId)
        {
            var game = await GetGameOrThrow(gameId);

            var player = game.Players.SingleOrDefault(p => p.Id == playerId);
            if (player == null)
            {
                throw new DomainException(DomainErrorCode.EntityMissing, $"Player with {gameId} is not in the game");
            }

            var discardPileCards = game.GetDiscardPile(ShitheadConstants.PileIdentifiers.DISCARD);
            var hand = player.GetHandPile();
            var faceUp = player.GetFaceUpPile();
            var faceDown = player.GetFaceDownPile();
            return new PlayerPilesResponse(discardPileCards.Cards, hand.Cards, faceUp.Cards, faceDown.Cards);
        }
    }
}