# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Build image with .NET 9 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Industrie/Industrie.csproj", "Industrie/"]
RUN dotnet restore "Industrie/Industrie.csproj"
COPY . .
WORKDIR "/src/Industrie"
RUN dotnet build "Industrie.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Industrie.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final production container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Industrie.dll"]
