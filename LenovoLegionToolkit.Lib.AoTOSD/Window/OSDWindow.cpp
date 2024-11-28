#include"OSDWindow.h"
#include"../Animation/FadeOutAnimation.h"

#include<sstream>

namespace Window = LenovoLegionToolkit::Lib::AoTOSD::Window;

Window::OSDWindow::OSDWindow(LPCWSTR className, LPCWSTR title, HINSTANCE hInstance) :
    LayeredWindow(className, title, hInstance, this->WINDOW_STYLE_FLAG)
{
    this->_hideAnimation = new Animation::FadeOutAnimation(255);
};

Window::OSDWindow::~OSDWindow() {
    delete this->_hideAnimation;
}

void Window::OSDWindow::Show() {
    if (this->_visible == false)
    {
        LayeredWindow::UpdateWindowPosition();
        ShowWindow(BasicWindow::GetHandle(), SW_SHOW);
        this->_visible = true;
    }

    if (this->_visibleDuration > 0)
    {
        SetTimer(BasicWindow::GetHandle(), this->TIMER_HIDE_ID, this->_visibleDuration, NULL);
        KillTimer(BasicWindow::GetHandle(), this->TIMER_OUT_ID);
        this->_hideAnimation->Reset(this);
    }
}

void Window::OSDWindow::Hide() {
    if (this->_visible == false)
    {
        return;
    }

    SetTimer(BasicWindow::GetHandle(), this->TIMER_OUT_ID, this->_hideAnimation->GetUpdateInterval(), NULL);
}

void Window::OSDWindow::SetVisibleDuration(int duration) {
    this->_visibleDuration = duration;
}

void Window::OSDWindow::AnimateOut() {
    bool isAnimateFinished = this->_hideAnimation->Animate(this);
    if (isAnimateFinished)
    {
        KillTimer(BasicWindow::GetHandle(), this->TIMER_OUT_ID);
        ShowWindow(BasicWindow::GetHandle(), SW_HIDE);
        this->_visible = false;
    }
}

LRESULT Window::OSDWindow::WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
    if (message == WM_TIMER)
    {
        switch (wParam)
        {
        case this->TIMER_HIDE_ID:
            this->Hide();
            KillTimer(hWnd, this->TIMER_HIDE_ID);
            break;
        case this->TIMER_OUT_ID:
            this->AnimateOut();
            break;
        }
    }
    return LayeredWindow::WndProc(hWnd, message, wParam, lParam);
}
