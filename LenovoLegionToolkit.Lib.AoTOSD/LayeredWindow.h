#pragma once

#include<Windows.h>
#include<gdiplus.h>

#include"Window.h"

namespace LenovoLegionToolkit::Lib::AoTOSD {

	class LayeredWindow : public Window {

	public:
		LayeredWindow(LPCWSTR className, LPCWSTR title, HINSTANCE hInstance = NULL,
			Gdiplus::Bitmap* bitmap = NULL, DWORD exStyles = NULL);
		virtual ~LayeredWindow() {};

		virtual void Show();
		virtual void Hide();

		Gdiplus::Bitmap* GetBitmap() const noexcept;
		virtual void SetBitmap(Gdiplus::Bitmap* bitmap);

		byte GetTransparency() const noexcept;
		virtual void SetTransparency(byte transparency);

		POINT GetPosition() const noexcept;
		int GetPositionX() const noexcept;
		int GetPositionY() const noexcept;
		virtual void SetPosition(POINT pos);
		virtual void SetPositionX(int x);
		virtual void SetPositionY(int y);

		int GetSizeWidth() const noexcept;
		int GetSizeHeight() const noexcept;

	protected:
		bool _visible;
		POINT _pos;
		SIZE _size;
		byte _transparency;
		Gdiplus::Bitmap* _bitmap;

		void UpdateWindow(RECT* dirtyRect = NULL);
		void UpdateTransparency();
		void UpdateWindowPosition();
		void SetWindowTopMost();

	}; // class LayeredWindow

} // namespace LenovoLegionToolkit::Lib::AoTOSD
