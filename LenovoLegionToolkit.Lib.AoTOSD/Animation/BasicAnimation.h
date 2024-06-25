#pragma once

namespace LenovoLegionToolkit::Lib::AoTOSD {

	class OSDWindow;

	class BasicAnimation {

	public:
		BasicAnimation(int speed);
		virtual ~BasicAnimation() {};

		virtual bool Animate(OSDWindow* window) = 0;
		virtual void Reset(OSDWindow* window) = 0;

		virtual int GetUpdateInterval() = 0;

	protected:
		int _speed;

	}; // class BasicAnimation

} // namespace LenovoLegionToolkit::Lib::AoTOSD
