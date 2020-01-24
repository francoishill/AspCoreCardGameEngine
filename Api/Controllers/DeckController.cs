using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspCoreCardGameEngine.Api.Domain.Services;
using AspCoreCardGameEngine.Api.Domain.Services.Requests;
using AspCoreCardGameEngine.Api.Domain.Services.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AspCoreCardGameEngine.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeckController : ControllerBase
    {
        private readonly IDeckService _deckService;

        public DeckController(IDeckService deckService)
        {
            _deckService = deckService;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<DeckResponse>>> CreateDecks([FromBody] CreateDecksRequest request)
        {
            var decks = await _deckService.CreateDecks(request);
            return decks.ToList();
        }
    }
}