msxsl.exe Core.Fakes.nuspec IncrementVersion.xsl -o Core.Fakes.NewVersion.nuspec
del Core.Fakes.nuspec
ren Core.Fakes.NewVersion.nuspec Core.Fakes.nuspec
nuget.exe pack Core.Fakes.nuspec

nuget.exe push  -source http://g-build-01.srv.int:8080/nuget Core.Fakes.*.nupkg