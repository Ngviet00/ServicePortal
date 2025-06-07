FROM mcr.microsoft.com/dotnet/aspnet:8.0.14 AS base
WORKDIR /app

EXPOSE 8080

COPY ./bin/Release/net8.0/publish_temp/ .

ENTRYPOINT ["dotnet", "ServicePortal.dll"]