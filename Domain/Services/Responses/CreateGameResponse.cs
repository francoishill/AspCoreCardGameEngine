using System;
using System.Linq;
using AspCoreCardGameEngine.Domain.Models.Database;

namespace AspCoreCardGameEngine.Domain.Services.Responses
{
    public class CreateGameResponse
    {
        public Guid Id { get; }
        public int TotalPlayers { get; set; }
        public int AvailablePlayers { get; set; }

        public CreateGameResponse(Game game)
        {
            Id = game.Id;
            TotalPlayers = game.Players.Count;
            AvailablePlayers = game.Players.Count(p => !p.Accepted);
        }
    }
}