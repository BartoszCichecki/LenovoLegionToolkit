#pragma once

#include"LayeredWindow.h"

namespace LenovoLegionToolkit::Lib::AoTOSD {

public ref class Test {

public:
	Test();
	~Test() {};

	void Hide();

private:

	LayeredWindow* window;

};

}
