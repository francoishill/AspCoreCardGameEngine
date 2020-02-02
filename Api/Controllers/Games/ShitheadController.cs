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
        public async Task<ActionResult<CreateGameResponse>> CreateShitheadGame(
            CreateShitheadGameRequest request)
        {
            return await _shitheadGameEngine.CreateGame(ShitheadGameConfig.Default, request);
        }

        [HttpPost("{gameId}/join")]
        public async Task<ActionResult<JoinGameResponse>> JoinShitheadGame(
            Guid gameId)
        {
            return await _shitheadGameEngine.JoinGame(gameId);
        }

        [HttpGet("{gameId}/state")]
        public async Task<ActionResult<GameStateResponse>> ShitheadGetGameState(
            Guid gameId,
            [FromHeader(Name = "X-Player-Id"), Required]
            Guid playerId)
        {
            return await _shitheadGameEngine.GetGameState(gameId, playerId);
        }

        [HttpPost("{gameId}/draw-from-deck")]
        public async Task<ActionResult> ShitheadDrawFromDeck(
            Guid gameId,
            [FromHeader(Name = "X-Player-Id"), Required]
            Guid playerId)
        {
            await _shitheadGameEngine.DrawFromDeck(gameId, playerId);
            return NoContent();
        }

        [HttpPost("{gameId}/play-cards")]
        public async Task<ActionResult> ShitheadPlayCards(
            Guid gameId,
            [FromHeader(Name = "X-Player-Id"), Required]
            Guid playerId,
            PlayShitheadCardsRequest request)
        {
            await _shitheadGameEngine.PlayCards(ShitheadGameConfig.Default, gameId, playerId, request);
            return NoContent();
        }

        [HttpPost("{gameId}/take-discard-pile")]
        public async Task<ActionResult> ShitheadTakeDiscardPile(
            Guid gameId,
            [FromHeader(Name = "X-Player-Id"), Required]
            Guid playerId)
        {
            await _shitheadGameEngine.TakeDiscardPile(ShitheadGameConfig.Default, gameId, playerId);
            return NoContent();
        }
    }
}