@echo off
pushd "%~dp0"
REM "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\msbuild.exe" $Env:GITHUB_WORKSPACE\DOOMSaveManager\DOOMSaveManager.csproj /p:Configuration=Release /p:Platform=x64
"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\msbuild.exe" /p:Configuration=Release /p:Platform=x86
:exit
popd
@echo on