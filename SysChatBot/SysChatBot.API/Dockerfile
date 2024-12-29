FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SysChatBot.API/SysChatBot.API.csproj", "SysChatBot.API/"]
COPY ["SysChatBot.Shared/SysChatBot.Shared.csproj", "SysChatBot.Shared/"]
RUN dotnet restore "SysChatBot.API/SysChatBot.API.csproj"
COPY . .
WORKDIR "/src/SysChatBot.API"
RUN dotnet build "SysChatBot.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SysChatBot.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SysChatBot.API.dll"]
