using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;

namespace Dietcode.Api.Core.Extenders
{
    public static class ForwardedHeadersExtensions
    {
        /// <summary>
        /// Habilita a resolução do IP real do cliente (HttpContext.Connection.RemoteIpAddress)
        /// a partir de X-Forwarded-For/X-Forwarded-Proto, necessária para o RateLimitAttribute
        /// funcionar corretamente atrás de proxy/load balancer/gateway.
        ///
        /// Exige KnownProxies ou KnownIPNetworks: sem restringir de quem a app aceita esses
        /// headers, qualquer cliente pode forjar X-Forwarded-For e trocar de "IP" a cada
        /// requisição, esvaziando o rate limit (justamente o que ele existe para evitar).
        /// </summary>
        public static IApplicationBuilder UseDietcodeForwardedHeaders(
            this IApplicationBuilder app,
            Action<ForwardedHeadersOptions> configureTrustedProxies)
        {
            ArgumentNullException.ThrowIfNull(configureTrustedProxies);

            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };

            configureTrustedProxies(options);

            if (options.KnownProxies.Count == 0 && options.KnownIPNetworks.Count == 0)
            {
                throw new InvalidOperationException(
                    "UseDietcodeForwardedHeaders exige KnownProxies ou KnownIPNetworks configurados " +
                    "em configureTrustedProxies. Sem isso, X-Forwarded-For é confiável de qualquer " +
                    "origem e o RateLimitAttribute pode ser contornado por IP forjado.");
            }

            return app.UseForwardedHeaders(options);
        }
    }
}
