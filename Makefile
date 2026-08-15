#WARNING: This Makefile is only for Skymu:Mac Legacy. Skymu:Mac Core is as easy as `dotnet build`. 

.PHONY: all restore forcerestore min core clean <pluginname>

BUILDER := msbuild

restore: Yggdrasil/obj/project.assets.json

forcerestore: nuget restore SkymuMacLegacy.sln

Yggdrasil/obj/project.assets.json:
	nuget restore SkymuMacLegacy.sln

min: restore
	$(BUILDER) SkymuMac/SkymuMacLegacy.csproj

core: min Stub

clean:
	rm -rf Plugins/bin
	find Plugins -maxdepth 2 -type d \( -name bin -o -name obj \) -exec rm -rf {} +
	rm -rf SkymuMac/bin SkymuMac/obj
	rm -rf Yggdrasil/bin Yggdrasil/obj

all: restore
	$(BUILDER) SkymuMacLegacy.sln

.DEFAULT: restore
	@echo 
	$(BUILDER) "Plugins/$@/$@-XamarinMac.csproj"
