#pragma once

#include"Window/OSDWindow.h"

namespace LenovoLegionToolkit::Lib::AoTOSD {

	public ref class Test {

	public:
		Test();
		~Test() { delete this->window; };

		void Show();

	private:

		Window::OSDWindow* window;

	};

}
