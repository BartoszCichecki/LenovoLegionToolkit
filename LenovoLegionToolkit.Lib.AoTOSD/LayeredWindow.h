#pragma once

#include<Windows.h>
#include<gdiplus.h>

#include"Window.h"

namespace LenovoLegionToolkit::Lib::AoTOSD {

class LayeredWindow : public Window {

public:
	LayeredWindow(LPCWSTR className, LPCWSTR title, HINSTANCE hInstance = NULL,
		Gdiplus::Bitmap* bitmap = NULL, DWORD exStyles = NULL);
	~LayeredWindow() {};

	void Show();
	void Hide();

protected:
	bool _visible;
	POINT _location;
	SIZE _size;
	byte _transparency;
	Gdiplus::Bitmap* _bitmap;

	void UpdateWindow(RECT* dirtyRect = NULL);
	void UpdateTransparency();
	void UpdateWindowPosition();

}; // class LayeredWindow

} // namespace LenovoLegionToolkit::Lib::AoTOSD
