FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY . .
# Build and publish a release
RUN dotnet publish ./NodeClient/NodeClient.csproj -o out -p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .
COPY ./NodeClient/config.json /App/
ENTRYPOINT dotnet NodeClient.dll