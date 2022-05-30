using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Utils;
using LenovoLegionToolkit.Lib.Listeners;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Automation
{
    public class AutomationProcessor
    {
        private readonly PowerStateListener _powerStateListener;
        private readonly AsyncLock _lock = new();

        private List<AutomationPipeline> _pipelines = new();
        private CancellationTokenSource? _cts;

        public static bool IsEnabled => AutomationSettings.Instance.IsEnabled;

        public AutomationProcessor(PowerStateListener powerAdapterListener)
        {
            _powerStateListener = powerAdapterListener;
            _powerStateListener.Changed += PowerAdapterListener_Changed;
        }

        private async void PowerAdapterListener_Changed(object? sender, EventArgs e) => await RunAsync();

        public async Task InitializeAsync()
        {
            using (await _lock.LockAsync().ConfigureAwait(false))
                _pipelines = AutomationSettings.Instance.Pipeliness;
        }

        public async Task<List<AutomationPipeline>> GetPipelinesAsync()
        {
            using (await _lock.LockAsync().ConfigureAwait(false))
                return _pipelines.Select(p => p.DeepCopy()).ToList();
        }

        public async Task ReloadPipelinesAsync(bool isEnabled, List<AutomationPipeline> pipelines)
        {
            using (await _lock.LockAsync().ConfigureAwait(false))
            {
                _pipelines = pipelines.Select(p => p.DeepCopy()).ToList();

                AutomationSettings.Instance.IsEnabled = isEnabled;
                AutomationSettings.Instance.Pipeliness = pipelines;
                AutomationSettings.Instance.Synchronize();
            }
        }

        private async Task RunAsync()
        {
            using (await _lock.LockAsync().ConfigureAwait(false))
            {
                _cts?.Cancel();

                if (!IsEnabled)
                    return;

                _cts = new CancellationTokenSource();

                var token = _cts.Token;

                foreach (var pipeline in _pipelines)
                {
                    if (token.IsCancellationRequested)
                        break;

                    await pipeline.RunAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
