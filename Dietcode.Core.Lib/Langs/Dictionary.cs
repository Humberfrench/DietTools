using System.Collections;

namespace Dietcode.Core.Lib.Langs
{
    public class Dictionary(string culture) : IEnumerable
    {
        public string Culture { get; } = culture ?? throw new ArgumentException("The culture argument is required.");

        private readonly Dictionary<string, string> _values = [];

        public void Add(string key, string value)
        {
            _values.Add(key, value);
        }

        public string? GetValue(string value)
        {
            if (value == null) { return null; }

            return _values.TryGetValue(value, out string? result) ? result : value;
        }

        public void AddRange(Dictionary dictionary)
        {
            foreach (KeyValuePair<string, string> value in dictionary._values)
            {
                _values.TryAdd(value.Key, value.Value);
            }
        }

        public void AddRange(Dictionary<string, string> dictionary)
        {
            foreach (KeyValuePair<string, string> value in dictionary)
            {
                _values.TryAdd(value.Key, value.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_values).GetEnumerator();
        }
    }
}
