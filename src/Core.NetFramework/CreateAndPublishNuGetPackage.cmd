@echo off

::set nuget_url=http://localhost/Xlent.Lever.Nuget.Service/api/v2/package
set nuget_url=http://fulcrum-nuget.azurewebsites.net/api/v2/package
set api_key=7b519fe3-ad97-460c-881c-ece381f5ae69 

echo.
echo READ THIS
echo.
echo 1. Build project (dll files are automatically put in lib folder)
echo 2. Change version number in Xlent.Lever.Libraries2.Core.nuspec
echo.
pause
echo.

del /q *.nupkg

NuGet.exe pack Xlent.Lever.Libraries2.Core.NetFramework.nuspec

nuget.exe push *.nupkg %api_key% -Source %nuget_url%

del /q *.nupkg

echo.
pause
