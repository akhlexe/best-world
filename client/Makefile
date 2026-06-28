SOLUTION := BestWorld.Client.sln
PROJECT := src/BestWorld.Client/BestWorld.Client.csproj
CONFIGURATION ?= Debug

.PHONY: restore build run clean

restore:
	dotnet restore $(SOLUTION)
	dotnet tool restore

build: restore
	dotnet build $(SOLUTION) -c $(CONFIGURATION)

run: restore
	dotnet run --project $(PROJECT)

clean:
	dotnet clean $(SOLUTION)
