#include"LayeredWindow.h"

namespace Window = LenovoLegionToolkit::Lib::AoTOSD::Window;

Window::LayeredWindow::LayeredWindow(LPCWSTR className, LPCWSTR title, HINSTANCE hInstance, DWORD exStyles) :
    BasicWindow(className, title, hInstance,
        CW_USEDEFAULT, CW_USEDEFAULT, 0, 0, NULL,
        WS_POPUP, WS_EX_LAYERED | exStyles),
    _transparency(255),
    _visible(false),
    _bitmap(NULL)
{
    this->SetWindowTopMost();
    return;
}

Window::LayeredWindow::~LayeredWindow() {
    delete this->_bitmap;
}

void Window::LayeredWindow::Show() {
    if (this->_visible == true) {
        return;
    }

    this->UpdateWindow();
    ShowWindow(BasicWindow::GetHandle(), SW_SHOW);
    this->_visible = true;
    return;
}

void Window::LayeredWindow::Hide() {
    if (this->_visible == false) {
        return;
    }

    ShowWindow(BasicWindow::GetHandle(), SW_HIDE);
    this->_visible = false;
    return;
}

Gdiplus::Bitmap* Window::LayeredWindow::GetBitmap() const noexcept {
    return this->_bitmap;
}

void Window::LayeredWindow::SetBitmap(Gdiplus::Bitmap* bitmap) {
    if (bitmap == NULL)
    {
        return;
    }

    if (this->_bitmap)
    {
        delete this->_bitmap;
    }

    this->_bitmap = bitmap;
    this->_size.cx = bitmap->GetWidth();
    this->_size.cy = bitmap->GetHeight();
    this->UpdateWindow();
    return;
}

byte Window::LayeredWindow::GetTransparency() const noexcept {
    return this->_transparency;
}

void Window::LayeredWindow::SetTransparency(byte transparency) {
    this->_transparency = transparency;
    this->UpdateTransparency();
    return;
}

POINT Window::LayeredWindow::GetPosition() const noexcept {
    return this->_pos;
}

int Window::LayeredWindow::GetPositionX() const noexcept {
    return this->_pos.x;
}

int Window::LayeredWindow::GetPositionY() const noexcept {
    return this->_pos.y;
}

void Window::LayeredWindow::SetPosition(POINT pos) {
    this->_pos = pos;
    this->UpdateWindowPosition();
    return;
}

void Window::LayeredWindow::SetPositionX(int x) {
    this->_pos.x = x;
    return;
}

void Window::LayeredWindow::SetPositionY(int y) {
    this->_pos.y = y;
    return;
}

int Window::LayeredWindow::GetSizeWidth() const noexcept {
    return this->_size.cx;
}

int Window::LayeredWindow::GetSizeHeight() const noexcept {
    return this->_size.cy;
}

void Window::LayeredWindow::UpdateWindow() {
    BLENDFUNCTION bFunc = { 0 };
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

    UPDATELAYEREDWINDOWINFO lwInfo = { 0 };
    lwInfo.cbSize = sizeof(UPDATELAYEREDWINDOWINFO);
    lwInfo.crKey = 0;
    lwInfo.dwFlags = ULW_ALPHA;
    lwInfo.hdcDst = screenDc;
    lwInfo.hdcSrc = sourceDc;
    lwInfo.pblend = &bFunc;
    lwInfo.pptDst = &(this->_pos);
    lwInfo.pptSrc = &pt;
    lwInfo.prcDirty = NULL;
    lwInfo.psize = &size;

    UpdateLayeredWindowIndirect(BasicWindow::GetHandle(), &lwInfo);

    SelectObject(sourceDc, hReplaced);
    DeleteDC(sourceDc);
    DeleteObject(hBmp);
    ReleaseDC(GetDesktopWindow(), screenDc);
    return;
}

void Window::LayeredWindow::UpdateTransparency() {
    BLENDFUNCTION bFunc = { 0 };
    bFunc.AlphaFormat = AC_SRC_ALPHA;
    bFunc.BlendFlags = 0;
    bFunc.BlendOp = AC_SRC_OVER;
    bFunc.SourceConstantAlpha = this->_transparency;

    UPDATELAYEREDWINDOWINFO lwInfo = { 0 };
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

    UpdateLayeredWindowIndirect(BasicWindow::GetHandle(), &lwInfo);
}

void Window::LayeredWindow::UpdateWindowPosition() {
    MoveWindow(
        BasicWindow::GetHandle(),
        this->_pos.x, this->_pos.y,
        this->_size.cx, this->_size.cy,
        FALSE
    );
}

void Window::LayeredWindow::SetWindowTopMost() {
    SetWindowPos(
        BasicWindow::GetHandle(), HWND_TOPMOST,
        this->_pos.x, this->_pos.y,
        this->_size.cx, this->_size.cy,
        NULL
    );
}
