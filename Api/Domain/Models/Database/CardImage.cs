using System;

namespace AspCoreCardGameEngine.Api.Domain.Models.Database
{
    public class CardImage : ICreatedDate, IUpdatedDate
    {
        public int Id { get; set; }

        public ImageTypeEnum Type { get; set; }
        public Uri Url { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}