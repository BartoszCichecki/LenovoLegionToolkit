@echo off

IF "%1"=="" (
	SET VERSION=0.0.1
) ELSE (
	SET VERSION=%1
)
	

SET PATH=%PATH%;"C:\Program Files (x86)\Inno Setup 6"

dotnet publish LenovoLegionToolkit.WPF -c release -o build /p:DebugType=None /p:FileVersion=%VERSION% /p:Version=%VERSION%
dotnet publish LenovoLegionToolkit.SpectrumTester -c release -o build /p:DebugType=None /p:FileVersion=%VERSION% /p:Version=%VERSION%

iscc make_installer.iss /DMyAppVersion=%VERSION%
