#!/bin/bash
dotnet --info
dotnet restore

# Build Library and Example
for path in env/*/*csproj; do
    dirname="$(dirname "${path}")"
    dotnet build ${dirname} -c Release
done
for path in src/*/*csproj; do
    dirname="$(dirname "${path}")"
    dotnet build ${dirname} -c Release
done

# Run Unit Tests
for path in test/*/*csproj; do
    dirname="$(dirname "${path}")"
    dotnet build ${dirname} -c Release
    dotnet run -p ${dirname} -c Release
done