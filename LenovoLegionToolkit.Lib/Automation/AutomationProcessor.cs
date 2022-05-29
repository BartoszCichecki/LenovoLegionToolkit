using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Listeners;
using NeoSmart.AsyncLock;

namespace LenovoLegionToolkit.Lib.Automation
{
    public class AutomationProcessor
    {
        private readonly PowerAdapterListener _powerAdapterListener = new();
        private readonly DisplayConfigurationListener _displayConfigurationListener = new();

        private readonly List<AutomationPipeline> _pipelines = new();

        private readonly AsyncLock _lock = new();
        private CancellationTokenSource? _cts;

        public AutomationProcessor()
        {
            var criteria = new List<AutomationPipelineCriteria>
            {
                AutomationPipelineCriteria.ACAdapterConnected,
                AutomationPipelineCriteria.DisplayConfigurationChanged,
            };
            var steps = new List<IAutomationStep>
            {
                new PowerModeAutomationStep(PowerModeState.Balance),
                new RefreshRateAutomationStep(new(165)),
            };
            _pipelines.Add(new(criteria, steps));
            var criteria2 = new List<AutomationPipelineCriteria>
            {
                AutomationPipelineCriteria.ACAdapterDisconnected,
                AutomationPipelineCriteria.DisplayConfigurationChanged,
            };
            var steps2 = new List<IAutomationStep>
            {
                new PowerModeAutomationStep(PowerModeState.Quiet),
                new RefreshRateAutomationStep(new(60)),
            };
            _pipelines.Add(new(criteria2, steps2));

            _powerAdapterListener.Changed += PowerAdapterListener_Changed;
            _displayConfigurationListener.Changed += DisplayConfigurationListener_Changed;

            _powerAdapterListener.Start();
            _displayConfigurationListener.Start();
        }

        private async void PowerAdapterListener_Changed(object? sender, EventArgs e) => await RunAsync();

        private async void DisplayConfigurationListener_Changed(object? sender, EventArgs e) => await RunAsync();

        private async Task RunAsync()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            var token = _cts.Token;

            using (await _lock.LockAsync().ConfigureAwait(false))
            {
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
