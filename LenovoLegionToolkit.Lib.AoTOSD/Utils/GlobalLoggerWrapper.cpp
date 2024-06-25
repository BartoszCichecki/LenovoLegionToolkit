#include"GlobalLoggerWrapper.h"

namespace Lib = LenovoLegionToolkit::Lib;
namespace Utils = LenovoLegionToolkit::Lib::AoTOSD::Utils;

using FormattableStringFactory = Runtime::CompilerServices::FormattableStringFactory;

void Utils::GlobalLoggerWrapper::Trace(const std::wstring& msg, const std::wstring& file, int lineNumber, const std::wstring& caller) {
	Lib::Utils::Log::Instance->Trace(
		ConvertWStringToManagedFormattableString(msg),
		nullptr,
		ConvertWStringToManagedString(file),
		lineNumber,
		ConvertWStringToManagedString(caller)
	);
	return;
}

String^ Utils::GlobalLoggerWrapper::ConvertWStringToManagedString(const std::wstring& wstr) {
	return gcnew String(wstr.c_str());
}

FormattableString^ Utils::GlobalLoggerWrapper::ConvertWStringToManagedFormattableString(const std::wstring& wstr) {
	String^ mstr = ConvertWStringToManagedString(wstr);
	return FormattableStringFactory::Create(mstr);
}
