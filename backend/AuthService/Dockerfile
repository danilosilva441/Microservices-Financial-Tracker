# =========================
# Stage 1 - Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copia os csproj
COPY SharedKernel/SharedKernel.csproj SharedKernel/
COPY backend/AuthService/AuthService.csproj backend/AuthService/

# 2. Restore
RUN dotnet restore backend/AuthService/AuthService.csproj

# 3. Copia o código-fonte
COPY SharedKernel/ SharedKernel/
COPY backend/AuthService/ backend/AuthService/

# 4. Publish
WORKDIR /src/backend/AuthService
RUN dotnet publish AuthService.csproj -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Stage 2 - Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "AuthService.dll"]
