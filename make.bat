@echo off

IF "%~1"=="" (
	SET COMPILER_PATH=msbuild
) ELSE (
	SET COMPILER_PATH="%~1"
)
IF "%~2"=="" (
	SET VERSION=0.0.1
) ELSE (
	SET VERSION=%2
)

SET PATH=%PATH%;"C:\Program Files (x86)\Inno Setup 6"

%COMPILER_PATH% LenovoLegionToolkit.sln -restore /p:Configuration=Release /p:DebugType=None /p:OutDir=%~dp0build\ /p:FileVersion=%VERSION% /p:Version=%VERSION% || exit /b

iscc make_installer.iss /DMyAppVersion=%VERSION% || exit /b
