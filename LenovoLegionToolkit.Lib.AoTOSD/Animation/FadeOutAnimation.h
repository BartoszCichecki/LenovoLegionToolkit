#pragma once

#include"BasicAnimation.h"

namespace LenovoLegionToolkit::Lib::AoTOSD::Animation {

    class FadeOutAnimation final : public BasicAnimation {

    public:
        FadeOutAnimation(int speed);
        ~FadeOutAnimation() {};

        bool Animate(Window::OSDWindow* window) override;
        void Reset(Window::OSDWindow* window) override;

        int GetUpdateInterval() override;

    private:
        int _interval;
        int _step;

    }; // class FadeOutAnimation

} // namespace LenovoLegionToolkit::Lib::AoTOSD::Animation
