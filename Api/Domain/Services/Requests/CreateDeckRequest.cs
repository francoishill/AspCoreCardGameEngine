namespace AspCoreCardGameEngine.Api.Domain.Services.Requests
{
    public class CreateDeckRequest
    {
        public int DeckCount { get; set; } = 1;

        public bool Shuffled { get; set; } = false;
    }
}