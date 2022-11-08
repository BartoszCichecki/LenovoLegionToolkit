﻿using System;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.WPF.Extensions;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Controls
{
    public abstract class AbstractToggleFeatureCardControl<T> : AbstractRefreshingControl where T : struct
    {
        private readonly IFeature<T> _feature = IoCContainer.Resolve<IFeature<T>>();

        private readonly CardControl _cardControl = new();

        private readonly CardHeaderControl _cardHeaderControl = new();

        private readonly ToggleSwitch _toggle = new();

        protected SymbolRegular Icon
        {
            get => _cardControl.Icon;
            set => _cardControl.Icon = value;
        }

        protected string Title
        {
            get => _cardHeaderControl.Title;
            set => _cardHeaderControl.Title = value;
        }

        protected string Subtitle
        {
            get => _cardHeaderControl.Subtitle;
            set => _cardHeaderControl.Subtitle = value;
        }

        protected abstract T OnState { get; }

        protected abstract T OffState { get; }

        protected AbstractToggleFeatureCardControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _toggle.Click += Toggle_Click;
            _toggle.Visibility = Visibility.Hidden;
            _toggle.Margin = new(8, 0, 0, 0);

            _cardHeaderControl.Accessory = _toggle;
            _cardControl.Header = _cardHeaderControl;
            _cardControl.Margin = new(0, 0, 0, 8);

            Content = _cardControl;
        }

        private async void Toggle_Click(object sender, RoutedEventArgs e) => await OnStateChange(_toggle, _feature);

        protected override async Task OnRefreshAsync()
        {
            if (!await _feature.IsSupportedAsync())
                throw new NotSupportedException();

            _toggle.IsChecked = OnState.Equals(await _feature.GetStateAsync());
        }

        protected override void OnFinishedLoading()
        {
            _toggle.Visibility = Visibility.Visible;

            MessagingCenter.Subscribe<T>(this, () => Dispatcher.InvokeTask(RefreshAsync));
        }

        protected virtual async Task OnStateChange(ToggleSwitch toggle, IFeature<T> feature)
        {
            if (IsRefreshing || toggle.IsChecked is null)
                return;

            var state = toggle.IsChecked.Value ? OnState : OffState;
            if (state.Equals(await feature.GetStateAsync()))
                return;

            await feature.SetStateAsync(state);
        }
    }
}
