#!/bin/bash
dotnet --info
dotnet restore src/Flatwhite.Core.sln --no-cache --verbosity minimal -s https://api.nuget.org/v3/index.json
dotnet build -c Release src/Flatwhite.Core.sln

for path in tests/*.Tests/*.csproj; do
    dotnet test -c Release ${path}
done