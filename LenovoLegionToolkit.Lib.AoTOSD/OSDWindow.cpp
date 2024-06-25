#include"OSDWindow.h"

#include<sstream>

namespace AoTOSD = LenovoLegionToolkit::Lib::AoTOSD;

AoTOSD::OSDWindow::OSDWindow(LPCWSTR className, LPCWSTR title, HINSTANCE hInstance) :
	LayeredWindow(className, title, hInstance, NULL, this->WINDOW_STYLE_FLAG) {};

AoTOSD::OSDWindow::~OSDWindow() {
	this->DeleteClones();
	return;
}

AoTOSD::LayeredWindow* AoTOSD::OSDWindow::Clone() {
	size_t newCloneNum = this->_clones.size() + 1;
	std::wostringstream cloneClassName;
	std::wostringstream cloneTitleName;
	cloneClassName << Window::GetClassName() << L":" << newCloneNum;
	cloneTitleName << Window::GetTitle() << L":" << newCloneNum;
	LayeredWindow* clone = new LayeredWindow(
		cloneClassName.str().c_str(),
		cloneTitleName.str().c_str(),
		Window::GetInstanceHandle(),
		LayeredWindow::GetBitmap(),
		GetWindowLong(Window::GetHandle(), GWL_EXSTYLE)
	);
	this->_clones.push_back(clone);
	return clone;
}

std::vector<AoTOSD::LayeredWindow*> AoTOSD::OSDWindow::GetClones() {
	return this->_clones;
}

void AoTOSD::OSDWindow::DeleteClones() {
	for (auto clone : this->_clones)
	{
		delete clone;
	}
	this->_clones.clear();
	return;
}

void AoTOSD::OSDWindow::Show() {
	if (this->_visible == false)
	{
		LayeredWindow::UpdateWindowPosition();
		ShowWindow(Window::GetHandle(), SW_SHOW);
		this->_visible = true;
	}

	this->ShowClones();
	if (this->_visibleDuration > 0)
	{
		SetTimer(Window::GetHandle(), this->TIMER_HIDE_ID, this->_visibleDuration, NULL);
		KillTimer(Window::GetHandle(), this->TIMER_OUT_ID);
		if (this->_hideAnimation)
		{
			this->_hideAnimation->Reset(this);
		}
	}
	return;
}

void AoTOSD::OSDWindow::Hide() {
	if (this->_visible == false)
	{
		return;
	}

	if (this->_hideAnimation)
	{
		SetTimer(Window::GetHandle(), this->TIMER_OUT_ID, this->_hideAnimation->GetUpdateInterval(), NULL);
	}
	else
	{
		ShowWindow(Window::GetHandle(), SW_HIDE);
		this->_visible = false;
		this->HideClones();
	}
	return;
}

void AoTOSD::OSDWindow::SetBitmap(Gdiplus::Bitmap* bitmap) {
	LayeredWindow::SetBitmap(bitmap);
	for (auto clone : this->_clones)
	{
		clone->SetBitmap(bitmap);
	}
	return;
}

void AoTOSD::OSDWindow::SetTransparency(byte transparency) {
	LayeredWindow::SetTransparency(transparency);
	for (auto clone : this->_clones)
	{
		clone->SetTransparency(transparency);
	}
	return;
}

void AoTOSD::OSDWindow::SetPosition(POINT pos) {
	LayeredWindow::SetPosition(pos);
	for (auto clone : this->_clones)
	{
		clone->SetPosition(pos);
	}
	return;
}

void AoTOSD::OSDWindow::SetPositionX(int x) {
	LayeredWindow::SetPositionX(x);
	for (auto clone : this->_clones)
	{
		clone->SetPositionX(x);
	}
	return;
}

void AoTOSD::OSDWindow::SetPositionY(int y) {
	LayeredWindow::SetPositionY(y);
	for (auto clone : this->_clones)
	{
		clone->SetPositionY(y);
	}
	return;
}

void AoTOSD::OSDWindow::SetHideAnimation(AoTOSD::BasicAnimation* hideAnimation) {
	if (this->_hideAnimation)
	{
		delete this->_hideAnimation;
	}
	this->_hideAnimation = hideAnimation;
	return;
}

void AoTOSD::OSDWindow::SetVisibleDuration(int duration) {
	this->_visibleDuration = duration;
	return;
}

void AoTOSD::OSDWindow::ShowClones() {
	for (auto clone : this->_clones)
	{
		clone->Show();
	}
	return;
}

void AoTOSD::OSDWindow::HideClones() {
	for (auto clone : this->_clones)
	{
		clone->Hide();
	}
	return;
}

void AoTOSD::OSDWindow::AnimateOut() {
	bool isAnimateFinished = this->_hideAnimation->Animate(this);
	if (isAnimateFinished)
	{
		KillTimer(Window::GetHandle(), this->TIMER_OUT_ID);
		ShowWindow(Window::GetHandle(), SW_HIDE);
		this->_visible = false;
		this->HideClones();
	}
	return;
}

LRESULT AoTOSD::OSDWindow::WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
	if (message == WM_TIMER)
	{
		switch (wParam)
		{
			case this->TIMER_HIDE_ID:
				this->Hide();
				KillTimer(hWnd, this->TIMER_HIDE_ID);
				break;
			case this->TIMER_OUT_ID:
				this->AnimateOut();
				break;
		}
	}
	return LayeredWindow::WndProc(hWnd, message, wParam, lParam);
}
