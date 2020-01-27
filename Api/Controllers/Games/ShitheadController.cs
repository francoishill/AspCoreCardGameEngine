using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AspCoreCardGameEngine.Domain.Services;
using AspCoreCardGameEngine.Domain.Services.Requests;
using AspCoreCardGameEngine.Domain.Services.Responses;
using Microsoft.AspNetCore.Mvc;

namespace AspCoreCardGameEngine.Api.Controllers.Games
{
    [ApiController]
    [Route("api/games/[controller]")]
    public class ShitheadController : ControllerBase
    {
        private readonly IShitheadGameEngine _shitheadGameEngine;

        public ShitheadController(IShitheadGameEngine shitheadGameEngine)
        {
            _shitheadGameEngine = shitheadGameEngine;
        }

        [HttpPost]
        public async Task<ActionResult<CreateGameResponse>> CreateShitheadGame(CreateShitheadGameRequest request)
        {
            return await _shitheadGameEngine.CreateGame(ShitheadGameConfig.Default, request);
        }

        [HttpPost("{id}/join")]
        public async Task<ActionResult<JoinGameResponse>> JoinShitheadGame(Guid id)
        {
            return await _shitheadGameEngine.JoinGame(id);
        }

        [HttpPost("{id}/draw-from-deck")]
        public async Task<ActionResult<DrawFromDeckResponse>> ShitheadDrawFromDeck(Guid id, [FromHeader(Name = "X-Player-Id"), Required] Guid playerId)
        {
            return await _shitheadGameEngine.DrawFromDeck(id, playerId);
        }
    }
}