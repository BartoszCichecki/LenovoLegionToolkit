#pragma once

#include<string>
#include<Windows.h>

namespace LenovoLegionToolkit::Lib::AoTOSD {

	class Window {

	public:
		Window(LPCWSTR className, LPCWSTR title = L"", HINSTANCE hInstance = NULL,
			int x = CW_USEDEFAULT, int y = CW_USEDEFAULT, int width = 0, int height = 0,
			UINT classStyle = NULL, DWORD style = NULL, DWORD exStyle = NULL,
			HWND parent = NULL, HMENU menu = NULL, HICON icon = NULL, HCURSOR cursor = NULL,
			HBRUSH background = (HBRUSH)(COLOR_WINDOW + 1));
		virtual ~Window();

		std::wstring GetClassName() const noexcept;
		std::wstring GetTitle() const noexcept;
		HWND GetHandle() const noexcept;
		HINSTANCE GetInstanceHandle() const noexcept;

	protected:
		static LRESULT CALLBACK StaticWndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);
		virtual LRESULT WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);

	private:
		std::wstring _className;
		std::wstring _title;
		HINSTANCE _hInstance;
		HWND _hWnd;

	}; // class Window

} // namespace LenovoLegionToolkit::Lib::AoTOSD


