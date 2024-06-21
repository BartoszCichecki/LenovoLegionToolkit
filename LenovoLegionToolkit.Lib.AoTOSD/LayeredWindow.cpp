#include"LayeredWindow.h"

// #include<dwmapi.h>

namespace AoTOSD = LenovoLegionToolkit::Lib::AoTOSD;

AoTOSD::LayeredWindow::LayeredWindow(LPCWSTR className, LPCWSTR title, HINSTANCE hInstance,
	Gdiplus::Bitmap* bitmap, DWORD exStyles) :
	Window(className, title, hInstance,
		CW_USEDEFAULT, CW_USEDEFAULT, 0, 0, NULL,
		WS_POPUP, WS_EX_LAYERED | exStyles),
	_transparency(255),
	_visible(false),
	_bitmap(bitmap)
{
	this->_size.cx = bitmap->GetWidth();
	this->_size.cy = bitmap->GetHeight();
	UpdateWindow();

	UpdateTransparency();

	this->_location.x = 128;
	this->_location.y = 128;
	UpdateWindowPosition();

	SetWindowPos(Window::Handle(), HWND_TOPMOST, this->_location.x, this->_location.y, this->_size.cx, this->_size.cy, NULL);
	
	return;
}

void AoTOSD::LayeredWindow::UpdateWindow(RECT* dirtyRect) {
    BLENDFUNCTION bFunc;
    bFunc.AlphaFormat = AC_SRC_ALPHA;
    bFunc.BlendFlags = 0;
    bFunc.BlendOp = AC_SRC_OVER;
    bFunc.SourceConstantAlpha = this->_transparency;

    HDC screenDc = GetDC(GetDesktopWindow());
    HDC sourceDc = CreateCompatibleDC(screenDc);

    HBITMAP hBmp;
    this->_bitmap->GetHBITMAP(Gdiplus::Color(0, 0, 0, 0), &hBmp);
    HGDIOBJ hReplaced = SelectObject(sourceDc, hBmp);

    POINT pt = { 0, 0 };
    SIZE size = {
        static_cast<LONG>(this->_bitmap->GetWidth()),
        static_cast<LONG>(this->_bitmap->GetHeight())
    };

    UPDATELAYEREDWINDOWINFO lwInfo;
    lwInfo.cbSize = sizeof(UPDATELAYEREDWINDOWINFO);
    lwInfo.crKey = 0;
    lwInfo.dwFlags = ULW_ALPHA;
    lwInfo.hdcDst = screenDc;
    lwInfo.hdcSrc = sourceDc;
    lwInfo.pblend = &bFunc;
    lwInfo.pptDst = &(this->_location);
    lwInfo.pptSrc = &pt;
    lwInfo.prcDirty = dirtyRect;
    lwInfo.psize = &size;

    UpdateLayeredWindowIndirect(Window::Handle(), &lwInfo);

    SelectObject(sourceDc, hReplaced);
    DeleteDC(sourceDc);
    DeleteObject(hBmp);
    ReleaseDC(GetDesktopWindow(), screenDc);
    return;
}

void AoTOSD::LayeredWindow::UpdateTransparency() {
    BLENDFUNCTION bFunc;
    bFunc.AlphaFormat = AC_SRC_ALPHA;
    bFunc.BlendFlags = 0;
    bFunc.BlendOp = AC_SRC_OVER;
    bFunc.SourceConstantAlpha = this->_transparency;

    UPDATELAYEREDWINDOWINFO lwInfo;
    lwInfo.cbSize = sizeof(UPDATELAYEREDWINDOWINFO);
    lwInfo.crKey = 0;
    lwInfo.dwFlags = ULW_ALPHA;
    lwInfo.hdcDst = NULL;
    lwInfo.hdcSrc = NULL;
    lwInfo.pblend = &bFunc;
    lwInfo.pptDst = NULL;
    lwInfo.pptSrc = NULL;
    lwInfo.prcDirty = NULL;
    lwInfo.psize = NULL;

    UpdateLayeredWindowIndirect(Window::Handle(), &lwInfo);
    return;
}

void AoTOSD::LayeredWindow::UpdateWindowPosition() {
    MoveWindow(Window::Handle(), this->_location.x, this->_location.y, this->_size.cx, this->_size.cy, FALSE);
    return;
}

void AoTOSD::LayeredWindow::Show() {
    if (this->_visible == true) {
        return;
    }

    UpdateWindow();
    ShowWindow(Window::Handle(), SW_SHOW);
    this->_visible = true;
    return;
}

void AoTOSD::LayeredWindow::Hide() {
    if (this->_visible == false) {
        return;
    }

    ShowWindow(Window::Handle(), SW_HIDE);
    this->_visible = false;
    return;
}
