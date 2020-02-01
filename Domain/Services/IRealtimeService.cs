using System.Threading.Tasks;

namespace AspCoreCardGameEngine.Domain.Services
{
    public interface IRealtimeService
    {
        Task OnGameMove(string gameId, string playerId);
    }
}