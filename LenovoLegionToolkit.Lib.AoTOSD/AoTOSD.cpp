#include"AoTOSD.h"
#include"Animation/FadeOutAnimation.h"
#include"Utils/BitmapConverter.h"
#include"Utils/DesktopInfos.h"
#include"Utils/GlobalLogger.h"

#include<gdiplus.h>

#define NOTIFICATIONWINDOWAOT_WINDOW_CLASSNAME L"LenovoLegionToolkit-OSDAOT"
#define NOTIFICATIONWINDOWAOT_WINDOW_TITLE L"LenovoLegionToolkit-OSDAOT"

using namespace System;

namespace AoTOSD = LenovoLegionToolkit::Lib::AoTOSD;

AoTOSD::NotificationWindowAoT::NotificationWindowAoT() {
    this->_window = new Window::OSDWindow(
        NOTIFICATIONWINDOWAOT_WINDOW_CLASSNAME,
        NOTIFICATIONWINDOWAOT_WINDOW_TITLE
    );
}

AoTOSD::NotificationWindowAoT::~NotificationWindowAoT() {
    delete this->_window;
}

void AoTOSD::NotificationWindowAoT::Show(Drawing::Bitmap^ bitmap, NotificationPosition pos, NotificationDuration duration) {
    this->_window->SetBitmap(Utils::BitmapConverter::Convert(bitmap));
    this->UpdateOSDWindowPosition(pos);
    this->UpdateOSDWindowVisibleDuration(duration);
    this->_window->Show();
}

void AoTOSD::NotificationWindowAoT::UpdateOSDWindowPosition(NotificationPosition pos) {
    POINT newPos = { 0 };
    int width = this->_window->GetSizeWidth();
    int height = this->_window->GetSizeHeight();
    RECT desktopWorkingArea = Utils::DesktopInfos::GetPrimaryDesktopWorkingArea();
    switch (pos)
    {
        case NotificationPosition::BottomLeft:
            newPos.x = desktopWorkingArea.left + this->Margin;
            newPos.y = desktopWorkingArea.bottom - height - this->Margin;
            break;
        case NotificationPosition::BottomCenter:
            newPos.x = (desktopWorkingArea.right - width) / 2;
            newPos.y = desktopWorkingArea.bottom - height - this->Margin;
            break;
        case NotificationPosition::BottomRight:
            newPos.x = desktopWorkingArea.right - width - this->Margin;
            newPos.y = desktopWorkingArea.bottom - height - this->Margin;
            break;
        case NotificationPosition::CenterLeft:
            newPos.x = desktopWorkingArea.left + this->Margin;
            newPos.y = (desktopWorkingArea.bottom - height) / 2;
            break;
        case NotificationPosition::Center:
            newPos.x = (desktopWorkingArea.right - width) / 2;
            newPos.y = (desktopWorkingArea.bottom - height) / 2;
            break;
        case NotificationPosition::CenterRight:
            newPos.x = desktopWorkingArea.right - width - this->Margin;
            newPos.y = (desktopWorkingArea.bottom - height) / 2;
            break;
        case NotificationPosition::TopLeft:
            newPos.x = desktopWorkingArea.left + this->Margin;
            newPos.y = desktopWorkingArea.top + this->Margin;
            break;
        case NotificationPosition::TopCenter:
            newPos.x = (desktopWorkingArea.right - width) / 2;
            newPos.y = desktopWorkingArea.top + this->Margin;
            break;
        case NotificationPosition::TopRight:
            newPos.x = desktopWorkingArea.right - width - this->Margin;
            newPos.y = desktopWorkingArea.top + this->Margin;
            break;
        default:
            Log() << L"Illegal notification position given, use center as default.";
            newPos.x = (desktopWorkingArea.right - width) / 2;
            newPos.y = (desktopWorkingArea.bottom - height) / 2;
            break;
    }
    this->_window->SetPosition(newPos);
}

void AoTOSD::NotificationWindowAoT::UpdateOSDWindowVisibleDuration(NotificationDuration duration) {
    switch (duration)
    {
        case NotificationDuration::Short:
            this->_window->SetVisibleDuration(500);
            break;
        case NotificationDuration::Normal:
            this->_window->SetVisibleDuration(1000);
            break;
        case NotificationDuration::Long:
            this->_window->SetVisibleDuration(2500);
            break;
        default:
            Log() << L"Illegal visible duration given, use normal as default.";
            this->_window->SetVisibleDuration(1500);
            break;
    }
}
