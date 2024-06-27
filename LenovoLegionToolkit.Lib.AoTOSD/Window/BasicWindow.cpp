#include"BasicWindow.h"
#include"../Utils/GlobalLogger.h"

#include<iostream>
#include<sstream>

namespace Window = LenovoLegionToolkit::Lib::AoTOSD::Window;

Window::BasicWindow::BasicWindow(LPCWSTR className, LPCWSTR title, HINSTANCE hInstance,
	int x, int y, int width, int height,
	UINT classStyle, DWORD style, DWORD exStyle) :
	_className(className),
	_title(title)
{
	if (hInstance == NULL)
	{
		hInstance = (HINSTANCE)GetModuleHandle(NULL);
	}
	this->_hInstance = hInstance;

	WNDCLASSEX wcex = { 0 };
	wcex.cbSize = sizeof(WNDCLASSEX);
	wcex.style = classStyle;
	wcex.lpfnWndProc = &BasicWindow::StaticWndProc;
	wcex.cbClsExtra = 0;
	wcex.cbWndExtra = 0;
	wcex.hInstance = hInstance;
	wcex.hIcon = NULL;
	wcex.hCursor = LoadCursor(NULL, IDC_ARROW);
	wcex.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
	wcex.lpszClassName = className;
	wcex.hIconSm = NULL;

	if (!RegisterClassEx(&wcex))
	{
		Log() << L"Failed to register AoT OSD window class. [LastError=" << GetLastError() << L"]";
		return;
	}

	if (_title.empty())
	{
		_title = className;
	}

	this->_hWnd = CreateWindowEx(
		exStyle, className, title, style,
		x, y, width, height,
		NULL, NULL, hInstance, this
	);
	if (this->_hWnd == NULL)
	{
		Log() << L"Failed to create AoT OSD window. [LastError=" << GetLastError() << L"]";
		return;
	}

	Log() << L"AoT OSD window has been created successfully.";
}

Window::BasicWindow::~BasicWindow() {
	DestroyWindow(this->_hWnd);
	UnregisterClass(this->_className.c_str(), this->_hInstance);
}

std::wstring Window::BasicWindow::GetClassName() const noexcept {
	return this->_className;
}

std::wstring Window::BasicWindow::GetTitle() const noexcept {
	return this->_title;
}

HWND Window::BasicWindow::GetHandle() const noexcept {
	return this->_hWnd;
}

HINSTANCE Window::BasicWindow::GetInstanceHandle() const noexcept {
	return this->_hInstance;
}

LRESULT CALLBACK Window::BasicWindow::StaticWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
	BasicWindow* window;

	if (message == WM_CREATE)
	{
		window = (BasicWindow*)((LPCREATESTRUCT)lParam)->lpCreateParams;
		SetWindowLongPtr(hWnd, GWLP_USERDATA, (LONG_PTR)window);
	}
	else
	{
		window = (BasicWindow*)GetWindowLongPtr(hWnd, GWLP_USERDATA);
		if (!window)
		{
			return DefWindowProc(hWnd, message, wParam, lParam);
		}
	}
	return window->WndProc(hWnd, message, wParam, lParam);
}

LRESULT Window::BasicWindow::WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
	return DefWindowProc(hWnd, message, wParam, lParam);
}
