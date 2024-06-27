#include"LayeredWindow.h"
#include "LayeredWindow.h"

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
    this->SetWindowTopMost();
	return;
}

void AoTOSD::LayeredWindow::Show() {
    if (this->_visible == true) {
        return;
    }

    this->UpdateWindow();
    ShowWindow(Window::GetHandle(), SW_SHOW);
    this->_visible = true;
    return;
}

void AoTOSD::LayeredWindow::Hide() {
    if (this->_visible == false) {
        return;
    }

    ShowWindow(Window::GetHandle(), SW_HIDE);
    this->_visible = false;
    return;
}

Gdiplus::Bitmap* AoTOSD::LayeredWindow::GetBitmap() const noexcept {
    return this->_bitmap;
}

void AoTOSD::LayeredWindow::SetBitmap(Gdiplus::Bitmap* bitmap) {
    if (bitmap == NULL)
    {
        return;
    }

    this->_bitmap = bitmap;
    this->_size.cx = bitmap->GetWidth();
    this->_size.cy = bitmap->GetHeight();
    this->UpdateWindow();
    return;
}

byte AoTOSD::LayeredWindow::GetTransparency() const noexcept {
    return this->_transparency;
}

void AoTOSD::LayeredWindow::SetTransparency(byte transparency) {
    this->_transparency = transparency;
    this->UpdateTransparency();
    return;
}

POINT AoTOSD::LayeredWindow::GetPosition() const noexcept {
    return this->_pos;
}

int AoTOSD::LayeredWindow::GetPositionX() const noexcept {
    return this->_pos.x;
}

int AoTOSD::LayeredWindow::GetPositionY() const noexcept {
    return this->_pos.y;
}

void AoTOSD::LayeredWindow::SetPosition(POINT pos) {
    this->_pos = pos;
    this->UpdateWindowPosition();
    return;
}

void AoTOSD::LayeredWindow::SetPositionX(int x) {
    this->_pos.x = x;
    return;
}

void AoTOSD::LayeredWindow::SetPositionY(int y) {
    this->_pos.y = y;
    return; 
}

int AoTOSD::LayeredWindow::GetSizeWidth() const noexcept {
    return this->_size.cx;
}

int AoTOSD::LayeredWindow::GetSizeHeight() const noexcept {
    return this->_size.cy;
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
    lwInfo.pptDst = &(this->_pos);
    lwInfo.pptSrc = &pt;
    lwInfo.prcDirty = dirtyRect;
    lwInfo.psize = &size;

    UpdateLayeredWindowIndirect(Window::GetHandle(), &lwInfo);

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

    UpdateLayeredWindowIndirect(Window::GetHandle(), &lwInfo);
    return;
}

void AoTOSD::LayeredWindow::UpdateWindowPosition() {
    MoveWindow(Window::GetHandle(), this->_pos.x, this->_pos.y, this->_size.cx, this->_size.cy, FALSE);
    return;
}

void AoTOSD::LayeredWindow::SetWindowTopMost() {
    SetWindowPos(
        Window::GetHandle(), HWND_TOPMOST,
        this->_pos.x, this->_pos.y,
        this->_size.cx, this->_size.cy,
        NULL
    );
    return;
}
