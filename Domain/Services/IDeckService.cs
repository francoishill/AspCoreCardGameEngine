using System.Collections.Generic;
using System.Threading.Tasks;
using AspCoreCardGameEngine.Domain.Services.Requests;
using AspCoreCardGameEngine.Domain.Services.Responses;

namespace AspCoreCardGameEngine.Domain.Services
{
    public interface IDeckService
    {
        Task<IEnumerable<DeckResponse>> CreateDecks(CreateDecksRequest request);
    }
}