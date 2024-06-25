#include"Window.h"

#include<iostream>

namespace AoTOSD = LenovoLegionToolkit::Lib::AoTOSD;

AoTOSD::Window::Window(LPCWSTR className, LPCWSTR title, HINSTANCE hInstance,
	int x, int y, int width, int height,
	UINT classStyle, DWORD style, DWORD exStyle,
	HWND parent, HMENU menu, HICON icon, HCURSOR cursor,
	HBRUSH background) :
	_className(className),
	_title(title)
{
	if (hInstance == NULL)
	{
		hInstance = (HINSTANCE)GetModuleHandle(NULL);
	}
	this->_hInstance = hInstance;

	if (cursor == NULL)
	{
		cursor = LoadCursor(NULL, IDC_ARROW);
	}

	WNDCLASSEX wcex = { 0 };
	wcex.cbSize = sizeof(WNDCLASSEX);
	wcex.style = classStyle;
	wcex.lpfnWndProc = &Window::StaticWndProc;
	wcex.cbClsExtra = 0;
	wcex.cbWndExtra = 0;
	wcex.hInstance = hInstance;
	wcex.hIcon = icon;
	wcex.hCursor = cursor;
	wcex.hbrBackground = background;
	wcex.lpszClassName = _className.c_str();
	wcex.hIconSm = NULL;

	if (!RegisterClassEx(&wcex))
	{
		// need to do error handling
		return;
	}

	if (_title.empty())
	{
		_title = className;
	}

	this->_hWnd = CreateWindowEx(
		exStyle, className, this->_title.c_str(), style,
		x, y, width, height,
		parent, menu, hInstance, this
	);
	if (this->_hWnd == NULL)
	{
		// need to do error handling
		return;
	}
	return;
}

AoTOSD::Window::~Window() {
	DestroyWindow(this->_hWnd);
	UnregisterClass(this->_className.c_str(), this->_hInstance);
	return;
}

std::wstring AoTOSD::Window::GetClassName() const noexcept {
	return this->_className;
}

std::wstring AoTOSD::Window::GetTitle() const noexcept {
	return this->_title;
}

HWND AoTOSD::Window::GetHandle() const noexcept {
	return this->_hWnd;
}

HINSTANCE AoTOSD::Window::GetInstanceHandle() const noexcept {
	return this->_hInstance;
}

LRESULT CALLBACK AoTOSD::Window::StaticWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
	Window* window;

	if (message == WM_CREATE)
	{
		window = (Window*)((LPCREATESTRUCT)lParam)->lpCreateParams;
		SetWindowLongPtr(hWnd, GWLP_USERDATA, (LONG_PTR)window);
	}
	else
	{
		window = (Window*)GetWindowLongPtr(hWnd, GWLP_USERDATA);
		if (!window)
		{
			return DefWindowProc(hWnd, message, wParam, lParam);
		}
	}
	return window->WndProc(hWnd, message, wParam, lParam);
}

LRESULT AoTOSD::Window::WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
	return DefWindowProc(hWnd, message, wParam, lParam);
}
