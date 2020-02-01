using System.Threading.Tasks;
using AspCoreCardGameEngine.Api.Hubs;
using AspCoreCardGameEngine.Domain.Services;
using Microsoft.AspNetCore.SignalR;

namespace AspCoreCardGameEngine.Api.ServiceImplementations
{
    public class RealtimeService : IRealtimeService
    {
        private readonly IHubContext<GameHub> _gameHub;

        public RealtimeService(
            IHubContext<GameHub> gameHub)
        {
            _gameHub = gameHub;
        }

        public async Task OnGameMove(string gameId, string playerId)
        {
            await _gameHub.Clients.All.SendAsync("GameMove", gameId, playerId);
        }
    }
}