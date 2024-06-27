#include"AoTOSD.h"
#include"Animation/FadeOutAnimation.h"
#include"Utils/GlobalLogger.h"

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

	this->window = new Window::OSDWindow(L"LLT_AoTOSD_Test_1", L"LLT_AoTOSD_Test_1", 0);
	this->window->SetBitmap(bitmap);
	this->window->SetVisibleDuration(1000);
	this->window->SetPosition({ 128, 128 });

	return;
}

void AoTOSD::Test::Show() {

	

	
	this->window->Show();
	return;
}
