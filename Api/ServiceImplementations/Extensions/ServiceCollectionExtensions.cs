using AspCoreCardGameEngine.Api.ServiceImplementations.Shithead;
using AspCoreCardGameEngine.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspCoreCardGameEngine.Api.ServiceImplementations.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServiceImplementations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IShuffler, Shuffler>();

            services.AddScoped<IDeckFactory, DeckFactory>();
            services.AddScoped<IShitheadGameEngine, ShitheadGameEngine>();
            services.AddScoped<IShitheadMoveValidator, ShitheadMoveValidator>();
            services.AddScoped<IShitheadPileLogic, ShitheadPileLogic>();
            services.AddScoped<IShitheadPlayerLogic, ShitheadPlayerLogic>();
            services.AddScoped<IRealtimeService, RealtimeService>();
        }
    }
}