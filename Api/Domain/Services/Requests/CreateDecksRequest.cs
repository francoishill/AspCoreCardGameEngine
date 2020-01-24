using System.ComponentModel.DataAnnotations;

namespace AspCoreCardGameEngine.Api.Domain.Services.Requests
{
    public class CreateDecksRequest
    {
        [Range(1, 1000)]
        public int DeckCount { get; set; } = 1;

        public bool Shuffled { get; set; } = false;

        public bool IncludeJokers { get; set; } = true;
    }
}