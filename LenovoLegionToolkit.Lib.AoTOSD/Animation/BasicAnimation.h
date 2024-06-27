#pragma once

namespace LenovoLegionToolkit::Lib::AoTOSD::Window { class OSDWindow; }

namespace LenovoLegionToolkit::Lib::AoTOSD::Animation {

	class BasicAnimation {

	public:
		BasicAnimation(int speed);
		virtual ~BasicAnimation() {};

		virtual bool Animate(Window::OSDWindow* window) = 0;
		virtual void Reset(Window::OSDWindow* window) = 0;

		virtual int GetUpdateInterval() = 0;

	protected:
		int _speed;

	}; // class BasicAnimation

} // namespace LenovoLegionToolkit::Lib::AoTOSD::Animation
