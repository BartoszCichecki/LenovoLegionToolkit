#pragma once

#include"OSDWindow.h"

namespace LenovoLegionToolkit::Lib::AoTOSD {

	public ref class Test {

	public:
		Test();
		~Test() { delete this->window; };

		void Show();

	private:

		OSDWindow* window;

	};

}
