FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /ScalableEventBusBuild
COPY . ./

WORKDIR /ScalableEventBusBuild/WorkerService
RUN dotnet restore
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /ScalableEventBus
COPY --from=build /ScalableEventBusBuild/WorkerService/bin/Release/netcoreapp3.1/publish/ ./

ENTRYPOINT ["dotnet", "ScalableEventBus.dll"]