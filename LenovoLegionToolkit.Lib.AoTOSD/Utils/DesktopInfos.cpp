#include"DesktopInfos.h"
#include"GlobalLogger.h"
#include"RECTCalcHelper.h"

#include<shellscalingapi.h>

namespace Utils = LenovoLegionToolkit::Lib::AoTOSD::Utils;

RECT Utils::DesktopInfos::GetPrimaryDesktopWorkingArea() {
    HMONITOR hMonitor = MonitorFromPoint({ 0 }, MONITOR_DEFAULTTOPRIMARY);
    MONITORINFO monitorInfo = { 0 };
    monitorInfo.cbSize = sizeof(MONITORINFO);
    if (!GetMonitorInfo(hMonitor, &monitorInfo))
    {
        Log() << L"Failed to get primary monitor info.";
        // need to return a default value
        return { 0 };
    }
    UINT dpiX, dpiY;
    if (!SUCCEEDED(GetDpiForMonitor(hMonitor, MDT_EFFECTIVE_DPI, &dpiX, &dpiY)))
    {
        Log() << L"Failed to get primary monitor dpi info.";
        // same, need to return a default value
        return { 0 };
    }
    RECT workingArea = monitorInfo.rcWork;
    double multiplierX = 96.0 / dpiX;
    double multiplierY = 96.0 / dpiY;
    
    RECT out = { 0 };
    out.left = workingArea.left;
    out.right = RECTWIDTH(workingArea) * multiplierX - out.left;
    out.top = workingArea.top;
    out.bottom = RECTHEIGHT(workingArea) * multiplierY - out.top;
    return out;
}
