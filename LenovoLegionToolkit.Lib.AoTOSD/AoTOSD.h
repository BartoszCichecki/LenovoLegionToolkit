#pragma once

#include"Window/OSDWindow.h"

namespace LenovoLegionToolkit::Lib::AoTOSD {

    public ref class Test {

    public:
        Test(::System::Drawing::Bitmap^ mbitmap);
        ~Test() { delete this->window; };

        void Show();

    private:

        Window::OSDWindow* window;

    };

}
