@echo off

SET PATH=%PATH%;"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin"
SET PATH=%PATH%;%~dp0\swigwin-4.1.1

msbuild ADLXCSharpBind /property:Configuration=Release /property:Platform=x64