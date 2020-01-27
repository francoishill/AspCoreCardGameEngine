using System.ComponentModel.DataAnnotations;

namespace AspCoreCardGameEngine.Domain.Services.Requests
{
    public class CreateShitheadGameRequest
    {
        [Range(1, 1000)]
        public int NumberPlayers { get; set; }
    }
}