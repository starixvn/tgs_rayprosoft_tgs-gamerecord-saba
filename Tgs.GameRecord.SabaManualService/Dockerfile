#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.SabaManualService/Tgs.GameRecord.SabaManualService.csproj", "Tgs.GameRecord.SabaManualService/"]
COPY ["Tgs.GameRecord.SabaService.Core/Tgs.GameRecord.SabaService.Core.csproj", "Tgs.GameRecord.SabaService.Core/"]
RUN dotnet restore "Tgs.GameRecord.SabaManualService/Tgs.GameRecord.SabaManualService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.SabaManualService"
RUN dotnet build "Tgs.GameRecord.SabaManualService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.SabaManualService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.SabaManualService.dll"]