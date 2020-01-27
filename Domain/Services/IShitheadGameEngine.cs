using System;
using System.Threading.Tasks;
using AspCoreCardGameEngine.Domain.Services.Requests;
using AspCoreCardGameEngine.Domain.Services.Responses;

namespace AspCoreCardGameEngine.Domain.Services
{
    public interface IShitheadGameEngine
    {
        Task<CreateGameResponse> CreateGame(ShitheadGameConfig config, CreateShitheadGameRequest request);
        Task<JoinGameResponse> JoinGame(Guid id);
        Task<DrawFromDeckResponse> DrawFromDeck(Guid id, Guid playerId);
    }
}