build: 
	dotnet build

run: 
	dotnet run

publish.mac:
	dotnet publish -c Release -r osx-x64
	@echo "For easier usage you can do \n\nPATH=$(CURDIR)/bin/Release/netcoreapp2/osx-x64/publish:$(PATH);"

publish.win:
	dotnet publish -c Release -r win10-x64

publish: publish.mac publish.win