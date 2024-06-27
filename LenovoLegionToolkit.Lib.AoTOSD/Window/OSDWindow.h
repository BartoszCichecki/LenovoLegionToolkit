#pragma once

#include<vector>

#include"../Animation/BasicAnimation.h"
#include"LayeredWindow.h"

namespace LenovoLegionToolkit::Lib::AoTOSD::Window {

    class OSDWindow : public LayeredWindow {

    public:
        OSDWindow(LPCWSTR className, LPCWSTR title, HINSTANCE hInstance = NULL);
        ~OSDWindow();

        void Show() override;
        void Hide() override;

        void SetVisibleDuration(int duration);

    protected:
        Animation::BasicAnimation* _hideAnimation = NULL;
        int _visibleDuration;

        void AnimateOut();

        LRESULT WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) override;

    private:
        static const DWORD WINDOW_STYLE_FLAG
            = WS_EX_TOOLWINDOW
            | WS_EX_NOACTIVATE
            | WS_EX_TOPMOST
            | WS_EX_TRANSPARENT;

        static const UINT_PTR TIMER_HIDE_ID = 0x0010;
        static const UINT_PTR TIMER_OUT_ID = 0x0100;

    }; // class OSDWindow

} // namespace LenovoLegionToolkit::Lib::AoTOSD