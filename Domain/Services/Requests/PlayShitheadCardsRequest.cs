using System.Collections.Generic;

namespace AspCoreCardGameEngine.Domain.Services.Requests
{
    public class PlayShitheadCardsRequest
    {
        public List<int> CardIds { get; set; }
    }
}