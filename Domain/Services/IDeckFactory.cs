using AspCoreCardGameEngine.Domain.Models.Database;
using AspCoreCardGameEngine.Domain.Services.Structs;

namespace AspCoreCardGameEngine.Domain.Services
{
    public interface IDeckFactory
    {
        void AddDeckCardsToPile(ShitheadGameConfig config, Pile pile, CreateDeckOptions options);
    }
}