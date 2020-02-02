using System;
using System.Threading.Tasks;
using AspCoreCardGameEngine.Domain.Services.Requests;
using AspCoreCardGameEngine.Domain.Services.Responses;

namespace AspCoreCardGameEngine.Domain.Services
{
    public interface IShitheadGameEngine
    {
        Task<CreateGameResponse> CreateGame(ShitheadGameConfig config, CreateShitheadGameRequest request);

        Task<JoinGameResponse> JoinGame(Guid gameId);

        Task DrawFromDeck(Guid gameId, Guid playerId);

        Task PlayCards(
            ShitheadGameConfig config,
            Guid gameId,
            Guid playerId,
            PlayShitheadCardsRequest request);

        Task TakeDiscardPile(
            ShitheadGameConfig config,
            Guid gameId,
            Guid playerId);

        Task<PlayerPilesResponse> GetPlayerPiles(Guid gameId, Guid playerId);
    }
}