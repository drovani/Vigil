#!/bin/bash
dotnet restore

# Build Library and Example
for path in src/*/*csproj; do
    dirname="$(dirname "${path}")"
    dotnet build ${dirname} -c Release
done

# Run Unit Tests
dotnet build test/Vigil.Domain.Tests/Vigil.Domain.Tests.csproj -f netcoreapp1.1 -c Release
dotnet run -p test/Vigil.Domain.Tests/Vigil.Domain.Tests.csproj -f netcoreapp1.1  -c Release