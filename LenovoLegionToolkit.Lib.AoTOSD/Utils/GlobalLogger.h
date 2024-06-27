#include<sstream>
#include<string>

#define Log()														\
	LenovoLegionToolkit::Lib::AoTOSD::Utils::GlobalLoggerCapture(	\
		__FILEW__,													\
		__LINE__,													\
		__FUNCTIONW__												\
	).Stream()

namespace LenovoLegionToolkit::Lib::AoTOSD::Utils {

	private ref class GlobalLogger {

	public:
		static void Trace(const std::wstring& msg, const std::wstring& file, int lineNumber, const std::wstring& caller);

	private:
		static ::System::String^ ConvertWStringToManagedString(const std::wstring& wstr);
		static ::System::FormattableString^ ConvertWStringToManagedFormattableString(const std::wstring& wstr);

	}; // private ref class GlobalLogger

	class GlobalLoggerCapture {

	public:
		GlobalLoggerCapture(const std::wstring& file, int lineNumber, const std::wstring& caller);
		~GlobalLoggerCapture();

		inline std::wostringstream& Stream() { return this->_woss; };

	private:
		std::wostringstream _woss;

		std::wstring _file;
		int _lineNumber;
		std::wstring _caller;

	}; // class GlobalLoggerCapture

} // namespace LenovoLegionToolkit::Lib::AoTOSD::Utils
