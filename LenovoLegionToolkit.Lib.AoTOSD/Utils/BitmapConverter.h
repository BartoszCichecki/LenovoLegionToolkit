#pragma once

#include<Windows.h>
#include<gdiplus.h>

namespace LenovoLegionToolkit::Lib::AoTOSD::Utils {

    private ref class BitmapConverter {

    public:
        static Gdiplus::Bitmap* Convert(::System::Drawing::Bitmap^ mBmp);

    }; // class BitmapConverter

} // namespace LenovoLegionToolkit::Lib::AoTOSD::Utils
