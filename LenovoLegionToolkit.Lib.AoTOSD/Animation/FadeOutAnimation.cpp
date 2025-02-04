#include"FadeOutAnimation.h"
#include"../Window/OSDWindow.h"

namespace Animation = LenovoLegionToolkit::Lib::AoTOSD::Animation;

Animation::FadeOutAnimation::FadeOutAnimation(int speed) :
    BasicAnimation(speed)
{
    int bestInaccuracy = 255;
    int bestInterval = 10;
    for (int i = 10; i <= 20; i++)
    {
        int si = this->_speed / i;
        int inaccuracy = 255 - 255 / si * si;
        if (inaccuracy < bestInaccuracy)
        {
            bestInaccuracy = inaccuracy;
            bestInterval = i;
        }
    }
    this->_interval = bestInterval;
    this->_step = 255 / (this->_speed / this->_interval);
    return;
}

bool Animation::FadeOutAnimation::Animate(Window::OSDWindow* window) {
    byte current = window->GetTransparency();
    int newTrans = current - this->_step;
    if (newTrans < 0)
    {
        newTrans = 0;
        return true;
    }
    window->SetTransparency(newTrans);
    return false;
}

void Animation::FadeOutAnimation::Reset(Window::OSDWindow* window) {
    window->SetTransparency(255);
    return;
}

int Animation::FadeOutAnimation::GetUpdateInterval() {
    return this->_interval;
}
