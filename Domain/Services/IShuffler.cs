using System.Collections.Generic;

namespace AspCoreCardGameEngine.Domain.Services
{
    public interface IShuffler
    {
        void Shuffle<T>(IList<T> list);
    }
}