msxsl.exe Core.nuspec IncrementVersion.xsl -o Core.NewVersion.nuspec
del Core.nuspec
ren Core.NewVersion.nuspec Core.nuspec
nuget.exe pack Core.nuspec
copy /Y Core.*.nupkg \\g-build-01\Packages
del \\g-build-01\Packages\g-build-01.cache.bin 2>nul
curl http://g-build-01:8080/nuget/Packages >nul