using System.Collections.Generic;

namespace AspCoreCardGameEngine.Api.Domain.Services
{
    public interface IShuffler
    {
        void Shuffle<T>(IList<T> list);
    }
}