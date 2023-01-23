using System;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF;

public readonly struct DashboardGroup
{
    public static readonly DashboardGroup[] DefaultGroups =
    {
        new(DashboardGroupType.Power, null, DashboardItem.PowerMode, DashboardItem.BatteryMode, DashboardItem.AlwaysOnUsb),
        new(DashboardGroupType.Graphics, null, DashboardItem.HybridMode, DashboardItem.DiscreteGpu),
        new(DashboardGroupType.Display, null, DashboardItem.Resolution, DashboardItem.RefreshRate, DashboardItem.DpiScale, DashboardItem.Hdr, DashboardItem.OverDrive),
        new(DashboardGroupType.Other, null, DashboardItem.Microphone, DashboardItem.FlipToStart, DashboardItem.TouchpadLock, DashboardItem.FnLock, DashboardItem.WinKeyLock)
    };

    public DashboardGroupType Type { get; }

    public string? CustomName { get; }

    public DashboardItem[] Items { get; }

    public DashboardGroup(DashboardGroupType type, string? customName, params DashboardItem[] items)
    {
        Type = type;
        CustomName = type == DashboardGroupType.Custom ? customName : null;
        Items = items;
    }

    public string GetName() => Type switch
    {
        DashboardGroupType.Power => Resource.DashboardPage_Power_Title,
        DashboardGroupType.Graphics => Resource.DashboardPage_Graphics_Title,
        DashboardGroupType.Display => Resource.DashboardPage_Display_Title,
        DashboardGroupType.Other => Resource.DashboardPage_Other_Title,
        DashboardGroupType.Custom => CustomName ?? string.Empty,
        _ => throw new InvalidOperationException($"Invalid type {Type}"),
    };
}
