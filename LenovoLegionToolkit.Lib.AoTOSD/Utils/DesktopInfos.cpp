#include"DesktopInfos.h"
#include"GlobalLogger.h"
#include"RECTCalcHelper.h"

#include<shellscalingapi.h>

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
    UINT dpiX, dpiY;
    if (!SUCCEEDED(GetDpiForMonitor(hMonitor, MDT_EFFECTIVE_DPI, &dpiX, &dpiY)))
    {
        Log() << L"Failed to get primary monitor dpi info.";
        out = GetSystemParameterWorkingArea();
        return out;
    }
    RECT workingArea = monitorInfo.rcWork;
    double multiplierX = 96.0 / dpiX;
    double multiplierY = 96.0 / dpiY;
    
    out.left = workingArea.left;
    out.right = RECTWIDTH(workingArea) * multiplierX - out.left;
    out.top = workingArea.top;
    out.bottom = RECTHEIGHT(workingArea) * multiplierY - out.top;
    return out;
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
