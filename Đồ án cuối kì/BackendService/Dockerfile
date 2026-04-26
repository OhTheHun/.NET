# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["BackendService/BackendService.csproj", "BackendService/"]
COPY ["BackendService.Core/BackendService.Core.csproj", "BackendService.Core/"]
COPY ["BackendService.Data/BackendService.Data.csproj", "BackendService.Data/"]
COPY ["BackendService.Model/BackendService.Model.csproj", "BackendService.Model/"]

RUN dotnet restore "BackendService/BackendService.csproj"

# Copy the remaining files and build
COPY . .
WORKDIR "/src/BackendService"
RUN dotnet build "BackendService.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "BackendService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "BackendService.dll"]
