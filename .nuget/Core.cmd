msxsl.exe Core.nuspec IncrementVersion.xsl -o Core.NewVersion.nuspec
del Core.nuspec
ren Core.NewVersion.nuspec Core.nuspec
nuget.exe pack Core.nuspec

nuget.exe push  -source http://g-build-01.srv.int:8080/nuget Core.*.nupkg

