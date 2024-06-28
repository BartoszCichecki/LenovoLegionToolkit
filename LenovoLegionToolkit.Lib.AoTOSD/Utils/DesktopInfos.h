#pragma once

#include<Windows.h>

namespace LenovoLegionToolkit::Lib::AoTOSD::Utils {

    class DesktopInfos {

    public:
        static RECT GetPrimaryDesktopWorkingArea();

    }; // class DesktopInfos

} // namespace LenovoLegionToolkit::Lib::AoTOSD::Utils
