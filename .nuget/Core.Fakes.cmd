msxsl.exe Core.Fakes.nuspec IncrementVersion.xsl -o Core.Fakes.NewVersion.nuspec
del Core.Fakes.nuspec
ren Core.Fakes.NewVersion.nuspec Core.Fakes.nuspec
nuget.exe pack Core.Fakes.nuspec
copy /Y Core.Fakes.*.nupkg \\g-build-01\Packages
del \\g-build-01\Packages\g-build-01.cache.bin 2>nul
curl http://g-build-01:8080/nuget/Packages >nul