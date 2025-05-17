﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.WPF.Extensions;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Windows.Settings;

public partial class NotificationsSettingsWindow
{
    private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();

    private IEnumerable<CardControl> Cards =>
    [
        _notificationPositionCard,
        _notificationDurationCard,
        _updateAvailableCard,
        _capsNumLockCard,
        _fnLockCard,
        _touchpadLockCard,
        _keyboardBacklightCard,
        _cameraLockCard,
        _microphoneCard,
        _powerModeCard,
        _refreshRateCard,
        _acAdapterCard,
        _smartKeyCard,
        _automationCard
    ];

    public NotificationsSettingsWindow()
    {
        InitializeComponent();

        _dontShowNotificationsToggle.IsChecked = _settings.Store.DontShowNotifications;
        _notificationAlwaysOnTopToggle.IsChecked = _settings.Store.NotificationAlwaysOnTop;
        _notificationOnAllScreensToggle.IsChecked = _settings.Store.NotificationOnAllScreens;

        _notificationPositionComboBox.SetItems(Enum.GetValues<NotificationPosition>(), _settings.Store.NotificationPosition, v => v.GetDisplayName());
        _notificationDurationComboBox.SetItems(Enum.GetValues<NotificationDuration>(), _settings.Store.NotificationDuration, v => v.GetDisplayName());

        _updateAvailableToggle.IsChecked = _settings.Store.Notifications.UpdateAvailable;
        _capsNumLockToggle.IsChecked = _settings.Store.Notifications.CapsNumLock;
        _fnLockToggle.IsChecked = _settings.Store.Notifications.FnLock;
        _touchpadLockToggle.IsChecked = _settings.Store.Notifications.TouchpadLock;
        _keyboardBacklightToggle.IsChecked = _settings.Store.Notifications.KeyboardBacklight;
        _cameraLockToggle.IsChecked = _settings.Store.Notifications.CameraLock;
        _microphoneToggle.IsChecked = _settings.Store.Notifications.Microphone;
        _powerModeToggle.IsChecked = _settings.Store.Notifications.PowerMode;
        _refreshRateToggle.IsChecked = _settings.Store.Notifications.RefreshRate;
        _acAdapterToggle.IsChecked = _settings.Store.Notifications.ACAdapter;
        _smartKeyToggle.IsChecked = _settings.Store.Notifications.SmartKey;
        _automationToggle.IsChecked = _settings.Store.Notifications.AutomationNotification;

        RefreshCards();
    }

    private void RefreshCards()
    {
        var notificationsDisabled = _dontShowNotificationsToggle.IsChecked ?? false;

        foreach (var card in Cards)
            card.IsEnabled = !notificationsDisabled;
    }

    private void DontShowNotificationsToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _dontShowNotificationsToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.DontShowNotifications = state.Value;
        _settings.SynchronizeStore();

        RefreshCards();
    }

    private void NotificationAlwaysOnTopToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _notificationAlwaysOnTopToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.NotificationAlwaysOnTop = state.Value;
        _settings.SynchronizeStore();
    }

    private void NotificationOnAllScreensToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _notificationOnAllScreensToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.NotificationOnAllScreens = state.Value;
        _settings.SynchronizeStore();
    }

    private void NotificationPositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_notificationPositionComboBox.TryGetSelectedItem(out NotificationPosition state))
            return;

        _settings.Store.NotificationPosition = state;
        _settings.SynchronizeStore();
    }

    private void NotificationDurationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_notificationDurationComboBox.TryGetSelectedItem(out NotificationDuration state))
            return;

        _settings.Store.NotificationDuration = state;
        _settings.SynchronizeStore();
    }

    private void CapsNumLockToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _capsNumLockToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.Notifications.CapsNumLock = state.Value;
        _settings.SynchronizeStore();
    }

    private void FnLockToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _fnLockToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.Notifications.FnLock = state.Value;
        _settings.SynchronizeStore();
    }

    private void TouchpadLockToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _touchpadLockToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.Notifications.TouchpadLock = state.Value;
        _settings.SynchronizeStore();
    }

    private void KeyboardBacklightToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _keyboardBacklightToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.Notifications.KeyboardBacklight = state.Value;
        _settings.SynchronizeStore();
    }

    private void CameraLockToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _cameraLockToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.Notifications.CameraLock = state.Value;
        _settings.SynchronizeStore();
    }

    private void MicrophoneToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _microphoneToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.Notifications.Microphone = state.Value;
        _settings.SynchronizeStore();
    }

    private void PowerModeToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _powerModeToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.Notifications.PowerMode = state.Value;
        _settings.SynchronizeStore();
    }

    private void RefreshRateToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _refreshRateToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.Notifications.RefreshRate = state.Value;
        _settings.SynchronizeStore();
    }

    private void ACAdapterToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _acAdapterToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.Notifications.ACAdapter = state.Value;
        _settings.SynchronizeStore();
    }

    private void SmartKeyToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _smartKeyToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.Notifications.SmartKey = state.Value;
        _settings.SynchronizeStore();
    }

    private void UpdateAvailableToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _updateAvailableToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.Notifications.UpdateAvailable = state.Value;
        _settings.SynchronizeStore();
    }

    private void AutomationToggle_Click(object sender, RoutedEventArgs e)
    {
        var state = _automationToggle.IsChecked;
        if (state is null)
            return;

        _settings.Store.Notifications.AutomationNotification = state.Value;
        _settings.SynchronizeStore();
    }
}
