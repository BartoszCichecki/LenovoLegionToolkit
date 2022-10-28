using System;
using System.Threading;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class DebounceDispatcher
    {
        private readonly TimeSpan _interval;
        private CancellationTokenSource? _cancellationTokenSource;

        public DebounceDispatcher(TimeSpan interval)
        {
            _interval = interval;
        }

        public async Task DebounceAsync(Func<Task> task)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new();

                var token = _cancellationTokenSource.Token;

                await Task.Delay(_interval, token)
                    .ContinueWith(async t =>
                    {
                        if (!t.IsCompletedSuccessfully)
                            return;

                        await task();
                    }, token);
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}
