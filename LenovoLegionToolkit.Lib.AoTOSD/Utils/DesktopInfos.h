#pragma once

#include<Windows.h>

namespace LenovoLegionToolkit::Lib::AoTOSD::Utils {

    class DesktopInfos {

    public:
        static RECT GetPrimaryDesktopWorkingArea();

    private:
        static RECT GetSystemParameterWorkingArea();

    }; // class DesktopInfos

} // namespace LenovoLegionToolkit::Lib::AoTOSD::Utils
