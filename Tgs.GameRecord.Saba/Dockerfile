FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.Saba/Tgs.GameRecord.Saba.csproj", "Tgs.GameRecord.Saba/"]
COPY ["Tgs.GameRecord.Saba.Common/Tgs.GameRecord.Saba.Common.csproj", "Tgs.GameRecord.Saba.Common/"]
COPY ["Tgs.GameRecord.Saba.Core/Tgs.GameRecord.Saba.Core.csproj", "Tgs.GameRecord.Saba.Core/"]
COPY ["Tgs.GameRecord.Saba.Repositories/Tgs.GameRecord.Saba.Repositories.csproj", "Tgs.GameRecord.Saba.Repositories/"]
RUN dotnet restore "./Tgs.GameRecord.Saba/Tgs.GameRecord.Saba.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.Saba"
RUN dotnet build "./Tgs.GameRecord.Saba.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.Saba.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.Saba.dll"]
