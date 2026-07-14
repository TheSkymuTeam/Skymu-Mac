.PHONY: all restore forcerestore min core clean <pluginname>

BUILDER := msbuild # dotnet build is accepted

restore: Yggdrasil/obj/project.assets.json

forcerestore: nuget restore SkymuMac.sln

Yggdrasil/obj/project.assets.json:
	nuget restore SkymuMac.sln

min: restore
	$(BUILDER) SkymuMac/SkymuMac.csproj

core: min Stub

clean:
	rm -rf Plugins/bin
	find Plugins -maxdepth 2 -type d \( -name bin -o -name obj \) -exec rm -rf {} +
	rm -rf SkymuMac/bin SkymuMac/obj
	rm -rf Yggdrasil/bin Yggdrasil/obj

all: restore
	$(BUILDER) SkymuMac.sln

.DEFAULT: restore
	@echo 
	$(BUILDER) "Plugins/$@/$@.csproj"
