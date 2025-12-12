# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj and restore
COPY . .
RUN dotnet restore "./Almustashar_app.csproj"
RUN dotnet publish "./Almustashar_app.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Render sets PORT env var. Bind Kestrel to it.
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Almustashar_app.dll"]
