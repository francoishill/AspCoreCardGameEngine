using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspCoreCardGameEngine.Domain.Services.Requests
{
    public class PlayShitheadCardsRequest
    {
        [Required]
        [MinLength(1)]
        public List<int> CardIds { get; set; }
    }
}