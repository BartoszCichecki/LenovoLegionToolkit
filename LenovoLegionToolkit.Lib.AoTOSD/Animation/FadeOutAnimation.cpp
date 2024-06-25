#include"FadeOutAnimation.h"
#include"../OSDWindow.h"

namespace AoTOSD = LenovoLegionToolkit::Lib::AoTOSD;

AoTOSD::FadeOutAnimation::FadeOutAnimation(int speed) :
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

bool AoTOSD::FadeOutAnimation::Animate(AoTOSD::OSDWindow* window) {
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

void AoTOSD::FadeOutAnimation::Reset(AoTOSD::OSDWindow* window) {
	window->SetTransparency(255);
	return;
}

int AoTOSD::FadeOutAnimation::GetUpdateInterval() {
	return this->_interval;
}
