msxsl.exe Core.Web.nuspec IncrementVersion.xsl -o Core.Web.NewVersion.nuspec
del Core.Web.nuspec
ren Core.Web.NewVersion.nuspec Core.Web.nuspec
nuget.exe pack Core.Web.nuspec
copy /Y Core.Web.*.nupkg \\g-build-01\Packages
del \\g-build-01\Packages\g-build-01.cache.bin 2>nul
curl http://g-build-01:8080/nuget/Packages >nul