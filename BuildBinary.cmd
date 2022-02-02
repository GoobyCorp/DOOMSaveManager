@echo off
pushd "%~dp0"
"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\msbuild.exe" /p:AllowUnsafeBlocks=true /p:Configuration=Release /p:Platform="Any CPU"
:exit
popd
@echo on