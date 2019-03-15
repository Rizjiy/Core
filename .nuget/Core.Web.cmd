msxsl.exe Core.Web.nuspec IncrementVersion.xsl -o Core.Web.NewVersion.nuspec
del Core.Web.nuspec
ren Core.Web.NewVersion.nuspec Core.Web.nuspec
nuget.exe pack Core.Web.nuspec
nuget.exe push  -source http://g-build-01.srv.int:8080/nuget Core.Web.*.nupkg
pause
