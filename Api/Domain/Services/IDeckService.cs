using System.Threading.Tasks;
using AspCoreCardGameEngine.Api.Domain.Services.Requests;
using AspCoreCardGameEngine.Api.Domain.Services.Responses;

namespace AspCoreCardGameEngine.Api.Domain.Services
{
    public interface IDeckService
    {
        Task<CreateDeckResponse> CreateDeck(CreateDeckRequest request);
    }
}