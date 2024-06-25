#include<sstream>
#include<string>

#using "LenovoLegionToolkit.Lib.dll"

#define LogTrace(msg) \
	LenovoLegionToolkit::Lib::AoTOSD::Utils::GlobalLoggerWrapper::Trace(msg, __FILEW__, __LINE__, __FUNCTIONW__)

using namespace System;

namespace LenovoLegionToolkit::Lib::AoTOSD::Utils {

	private ref class GlobalLoggerWrapper {

	public:
		static void Trace(const std::wstring& msg, const std::wstring& file, int lineNumber, const std::wstring& caller);

	private:
		static String^ ConvertWStringToManagedString(const std::wstring& wstr);
		static FormattableString^ ConvertWStringToManagedFormattableString(const std::wstring& wstr);

	}; // private ref class GlobalLoggerWrapper::Utils

} // namespace LenovoLegionToolkit::Lib::AoTOSD
