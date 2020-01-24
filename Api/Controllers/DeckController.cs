using System.Threading.Tasks;
using AspCoreCardGameEngine.Api.Domain.Services;
using AspCoreCardGameEngine.Api.Domain.Services.Requests;
using AspCoreCardGameEngine.Api.Domain.Services.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspCoreCardGameEngine.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeckController : ControllerBase
    {
        private readonly ILogger<DeckController> _logger;
        private readonly IDeckService _deckService;

        public DeckController(ILogger<DeckController> logger, IDeckService deckService)
        {
            _logger = logger;
            _deckService = deckService;
        }

        [HttpPost]
        public async Task<ActionResult<CreateDeckResponse>> CreateDeck([FromBody] CreateDeckRequest request)
        {
            var deck = await _deckService.CreateDeck(request);

            _logger.LogDebug($"Deck created with Id {deck.Id}");

            return deck;
        }
    }
}