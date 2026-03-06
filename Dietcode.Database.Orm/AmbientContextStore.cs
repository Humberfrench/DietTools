using System.Collections.Concurrent;
using Dietcode.Database.Domain;

namespace Dietcode.Database.Orm.Context
{
    public sealed class AmbientContextStore : IAmbientContextStore
    {
        private sealed class ScopeState
        {
            public ConcurrentDictionary<string, object> Items { get; } = new();
        }

        private static readonly AsyncLocal<ScopeState?> _current = new();

        public bool TryGet<T>(string key, out T value)
        {
            value = default!;
            var state = _current.Value;
            if (state is null) return false;

            if (state.Items.TryGetValue(key, out var obj) && obj is T cast)
            {
                value = cast;
                return true;
            }

            return false;
        }

        public void Set(string key, object value)
        {
            var state = _current.Value ??= new ScopeState();
            state.Items[key] = value;
        }

        public IDisposable BeginScope()
        {
            var previous = _current.Value;
            _current.Value = new ScopeState();

            return new DisposableAction(() => _current.Value = previous);
        }

        private sealed class DisposableAction : IDisposable
        {
            private readonly Action _action;
            private int _disposed;

            public DisposableAction(Action action) => _action = action;

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 0)
                    _action();
            }
        }
    }
}
