#pragma once

#include"Window/OSDWindow.h"

namespace LenovoLegionToolkit::Lib::AoTOSD {

    public ref class NotificationWindowAoT {

    public:
        NotificationWindowAoT();
        ~NotificationWindowAoT();

        void Show(::System::Drawing::Bitmap^ bitmap, NotificationPosition pos, NotificationDuration duration);

    private:
        Window::OSDWindow* _window;
        static const int Margin = 16;

        void UpdateOSDWindowPosition(NotificationPosition pos);
        void UpdateOSDWindowVisibleDuration(NotificationDuration duration);

    }; // public ref class NotificationWindowAoT

} // namespace LenovoLegionToolkit::Lib::AoTOSD
