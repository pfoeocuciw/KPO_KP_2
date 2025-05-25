FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Gateway/Gateway.csproj Gateway/
RUN dotnet restore Gateway/Gateway.csproj

COPY Gateway/. ./Gateway/
WORKDIR /src/Gateway
RUN dotnet publish Gateway.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "Gateway.dll"]