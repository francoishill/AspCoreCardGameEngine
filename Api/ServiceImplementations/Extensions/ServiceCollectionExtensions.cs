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

            services.AddScoped<IDeckService, DeckService>();
        }
    }
}