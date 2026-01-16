using Dietcode.Api.Core.MiddleObjects;
using Dietcode.Api.Core.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dietcode.Api.Core.Extenders
{
    public static class ApiLoggingExtensions
    {
        public static IServiceCollection AddApiLogging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ApiLoggingOptions>(
                configuration.GetSection("ApiLogging"));

            return services;
        }

        public static IApplicationBuilder UseApiLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApiLoggingMiddleware>();
        }
    }
}
