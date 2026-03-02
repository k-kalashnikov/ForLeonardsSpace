# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ARG ASPNETCORE_ENVIRONMENT

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Masofa.Web.Monolith/Masofa.Web.Monolith.csproj", "Masofa.Web.Monolith/"]
COPY ["Masofa.BusinessLogic/Masofa.BusinessLogic.csproj", "Masofa.BusinessLogic/"]
COPY ["Masofa.Common/Masofa.Common.csproj", "Masofa.Common/"]
COPY ["Masofa.DataAccess/Masofa.DataAccess.csproj", "Masofa.DataAccess/"]
COPY ["Masofa.Client.Copernicus/Masofa.Client.Copernicus.csproj", "Masofa.Client.Copernicus/"]
COPY ["Masofa.Client.EarthExplorer/Masofa.Client.EarthExplorer.csproj", "Masofa.Client.EarthExplorer/"]
COPY ["Masofa.Depricated.DataAccess/Masofa.Depricated.DataAccess.csproj", "Masofa.Depricated.DataAccess/"]
COPY ["Masofa.Client.Era5/Masofa.Client.Era5.csproj", "Masofa.Client.Era5/"]
COPY ["Masofa.Client.OneId/Masofa.Client.OneId.csproj", "Masofa.Client.OneId/"]
COPY ["Masofa.Client.IBMWeather/Masofa.Client.IBMWeather.csproj", "Masofa.Client.IBMWeather/"]
COPY ["Masofa.Client.Ugm/Masofa.Client.Ugm.csproj", "Masofa.Client.Ugm/"]
COPY ["Masofa.Client.Qwen/Masofa.Client.Qwen.csproj", "Masofa.Client.Qwen/"]

RUN dotnet restore "./Masofa.Web.Monolith/Masofa.Web.Monolith.csproj"

COPY ["Masofa.Web.Monolith", "Masofa.Web.Monolith/"]
COPY ["Masofa.BusinessLogic", "Masofa.BusinessLogic/"]
COPY ["Masofa.Common", "Masofa.Common/"]
COPY ["Masofa.DataAccess", "Masofa.DataAccess/"]
COPY ["Masofa.Client.Copernicus", "Masofa.Client.Copernicus/"]
COPY ["Masofa.Client.EarthExplorer", "Masofa.Client.EarthExplorer/"]
COPY ["Masofa.Depricated.DataAccess", "Masofa.Depricated.DataAccess/"]
COPY ["Masofa.Client.Era5", "Masofa.Client.Era5/"]
COPY ["Masofa.Client.OneId", "Masofa.Client.OneId/"]
COPY ["Masofa.Client.IBMWeather", "Masofa.Client.IBMWeather/"]
COPY ["Masofa.Client.Ugm", "Masofa.Client.Ugm/"]
COPY ["Masofa.Client.Qwen", "Masofa.Client.Qwen/"]



WORKDIR "/src/Masofa.Web.Monolith"
RUN dotnet build "./Masofa.Web.Monolith.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Masofa.Web.Monolith.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Masofa.Web.Monolith/appsettings.$ASPNETCORE_ENVIRONMENT.json .
ENV GDAL_DATA=/app/runtimes/linux-x64/native/gdal-data
ENV PROJ_LIB=/app/runtimes/linux-x64/native/maxrev.gdal.core.libshared
ENTRYPOINT ["dotnet", "Masofa.Web.Monolith.dll"]
#ENTRYPOINT ["tail", "-f", "/dev/null"]