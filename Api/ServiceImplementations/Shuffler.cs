using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using AspCoreCardGameEngine.Domain.Services;

namespace AspCoreCardGameEngine.Api.ServiceImplementations
{
    public class Shuffler : IShuffler
    {
        public void Shuffle<T>(IList<T> list)
        {
            var provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}