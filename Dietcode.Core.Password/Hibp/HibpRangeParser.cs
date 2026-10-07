namespace Dietcode.Core.Password.Hibp;

public sealed class HibpRangeParser : IHibpRangeParser
{
    public long FindOccurrences(string response, string expectedSuffix)
    {
        if (string.IsNullOrEmpty(response))
            return 0;

        foreach (var rawLine in response.Split('\n'))
        {
            var line = rawLine.Trim();
            if (line.Length == 0)
                continue;

            var separatorIndex = line.IndexOf(':');
            if (separatorIndex < 0)
                continue;

            var suffix = line[..separatorIndex];
            var countText = line[(separatorIndex + 1)..];

            if (!long.TryParse(countText, out var count) || count == 0)
                continue;

            if (string.Equals(suffix, expectedSuffix, StringComparison.OrdinalIgnoreCase))
                return count;
        }

        return 0;
    }
}
