

release: 
ifeq ($(strip $(version)),)
	@echo "\033[31mERROR:\033[0;39m No version provided."
	@echo "\033[1;30mmake pack version=1.0.0\033[0;39m"
else
	dotnet pack --verbosity quiet --nologo -c Release /p:PackageVersion=$(version) src
	@echo packed $(version)
	nuget push src/bin/Release/TimoStamm.WebSockets.Controller.$(version).nupkg  -Source https://www.nuget.org
endif

