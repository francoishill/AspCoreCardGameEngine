using System;

namespace AspCoreCardGameEngine.Domain.Services.Responses
{
    public class JoinGameResponse
    {
        public Guid PlayerId { get; set; }

        public JoinGameResponse(Guid playerId)
        {
            PlayerId = playerId;
        }
    }
}