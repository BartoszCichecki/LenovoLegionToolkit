#include"GlobalLogger.h"

#using "LenovoLegionToolkit.Lib.dll"

using namespace System;
using namespace System::Runtime::CompilerServices;

namespace Lib = LenovoLegionToolkit::Lib;
namespace Utils = LenovoLegionToolkit::Lib::AoTOSD::Utils;

void Utils::GlobalLogger::Trace(const std::wstring& msg, const std::wstring& file, int lineNumber, const std::wstring& caller) {
    Lib::Utils::Log::Instance->Trace(
        ConvertWStringToManagedFormattableString(msg),
        nullptr,
        ConvertWStringToManagedString(file),
        lineNumber,
        ConvertWStringToManagedString(caller)
    );
    return;
}

String^ Utils::GlobalLogger::ConvertWStringToManagedString(const std::wstring& wstr) {
    return gcnew String(wstr.c_str());
}

FormattableString^ Utils::GlobalLogger::ConvertWStringToManagedFormattableString(const std::wstring& wstr) {
    String^ mstr = ConvertWStringToManagedString(wstr);
    return FormattableStringFactory::Create(mstr);
}

Utils::GlobalLoggerCapture::GlobalLoggerCapture(const std::wstring& file, int lineNumber, const std::wstring& caller) :
    _woss(),
    _file(file),
    _lineNumber(lineNumber),
    _caller(caller) {};

Utils::GlobalLoggerCapture::~GlobalLoggerCapture() {
    GlobalLogger::Trace(this->_woss.str(), this->_file, this->_lineNumber, this->_caller);
    return;
}
