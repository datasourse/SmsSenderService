using Microsoft.Extensions.DependencyInjection;

namespace WebApplication.Extensions
{
    public static class ServicesExtensions
    {
        public static T GetScopedService<T>(this IServiceScopeFactory serviceScopeFactory)
        {
            return serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<T>();
        }
    }
}