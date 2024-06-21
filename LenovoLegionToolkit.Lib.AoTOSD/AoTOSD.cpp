#include"AoTOSD.h"

#include<gdiplus.h>

namespace AoTOSD = LenovoLegionToolkit::Lib::AoTOSD;

AoTOSD::Test::Test() {
	Gdiplus::GdiplusStartupInput gdiplusStartupInput;
	ULONG_PTR gdiplusToken;
	Gdiplus::GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);

	Gdiplus::Image* image = new Gdiplus::Image(L"eject.png");
	Gdiplus::Bitmap* bitmap = new Gdiplus::Bitmap(image->GetWidth(), image->GetHeight(), PixelFormat32bppARGB);

	Gdiplus::Graphics* graphics = Gdiplus::Graphics::FromImage(bitmap);

	graphics->DrawImage(image, 0, 0, image->GetWidth(), image->GetHeight());

	this->window = new LayeredWindow(L"AoTTest_1", L"AoTTest_1", 0, bitmap, 0);
	this->window->Show();

	return;
}

void AoTOSD::Test::Hide() {
	this->window->Hide();
	delete this->window;
	this->window = NULL;
	return;
}
