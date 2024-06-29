#include"DesktopInfos.h"
#include"GlobalLogger.h"

namespace Utils = LenovoLegionToolkit::Lib::AoTOSD::Utils;

RECT Utils::DesktopInfos::GetPrimaryDesktopWorkingArea() {
    RECT out = { 0 };
    HMONITOR hMonitor = MonitorFromPoint({ 0 }, MONITOR_DEFAULTTOPRIMARY);
    MONITORINFO monitorInfo = { 0 };
    monitorInfo.cbSize = sizeof(MONITORINFO);
    if (!GetMonitorInfo(hMonitor, &monitorInfo))
    {
        Log() << L"Failed to get primary monitor info.";
        out = GetSystemParameterWorkingArea();
        return out;
    }
    return monitorInfo.rcWork;
}

RECT Utils::DesktopInfos::GetSystemParameterWorkingArea() {
    RECT out = { 0 };
    if (!SystemParametersInfo(SPI_GETWORKAREA, 0, &out, 0))
    {
        Log() << L"Cannot get system-wide working area. [LastError=" << GetLastError() << L"]";
        out = { 0 };
    }
    return out;
}
