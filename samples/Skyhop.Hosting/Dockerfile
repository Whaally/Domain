FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["samples/Skyhop.Hosting/Skyhop.Hosting.csproj", "samples/Skyhop.Hosting/"]
RUN dotnet restore "samples/Skyhop.Hosting/Skyhop.Hosting.csproj"
COPY . .
WORKDIR "/src/samples/Skyhop.Hosting"
RUN dotnet build "Skyhop.Hosting.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Skyhop.Hosting.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Skyhop.Hosting.dll"]
