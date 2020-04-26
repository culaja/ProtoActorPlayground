FROM mcr.microsoft.com/dotnet/core/runtime:3.1

COPY WorkerService/bin/Release/netcoreapp3.1/publish/ /WorkerService

WORKDIR /WorkerService

ENTRYPOINT ["dotnet", "ScalableEventBus.dll"]