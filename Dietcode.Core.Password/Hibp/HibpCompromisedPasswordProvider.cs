using Dietcode.Core.Password.Abstractions;
using Dietcode.Core.Password.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Dietcode.Core.Password.Hibp;

public sealed class HibpCompromisedPasswordProvider : ICompromisedPasswordProvider
{
    internal const string ProviderName = "HIBP";

    private readonly HibpClient _client;
    private readonly IPasswordHashService _hashService;
    private readonly IHibpRangeParser _parser;
    private readonly IMemoryCache _cache;
    private readonly HibpOptions _options;

    public HibpCompromisedPasswordProvider(
        HibpClient client,
        IPasswordHashService hashService,
        IHibpRangeParser parser,
        IMemoryCache cache,
        IOptions<HibpOptions> options)
    {
        _client = client;
        _hashService = hashService;
        _parser = parser;
        _cache = cache;
        _options = options.Value;
    }

    public async Task<PasswordBreachResult> CheckAsync(string password, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(password))
            return PasswordBreachResult.InvalidInput(ProviderName);

        if (!_options.Enabled)
            return PasswordBreachResult.Safe(ProviderName);

        var range = _hashService.CreateRange(password);
        var cacheKey = CacheKey(range.Prefix);

        if (!_cache.TryGetValue(cacheKey, out string? response))
        {
            response = await _client.GetRangeAsync(range.Prefix, cancellationToken);

            // Nunca cacheia falha: uma indisponibilidade transitória não deve
            // "travar" o prefixo como indisponível por CacheMinutes inteiros.
            if (response is not null)
                _cache.Set(cacheKey, response, TimeSpan.FromMinutes(_options.CacheMinutes));
        }

        if (response is null)
            return PasswordBreachResult.Unavailable(ProviderName);

        var occurrences = _parser.FindOccurrences(response, range.Suffix);

        return occurrences > 0
            ? PasswordBreachResult.Compromised(occurrences, ProviderName)
            : PasswordBreachResult.Safe(ProviderName);
    }

    private static string CacheKey(string prefix) => $"hibp:range:{prefix}";
}
