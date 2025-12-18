# =========================
# Stage 1 - Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copia os csproj primeiro
COPY ["../../SharedKernel/SharedKernel.csproj", "../../SharedKernel/"]
COPY ["AuthService.csproj", "./"]

# 2. Restore
RUN dotnet restore "AuthService.csproj"

# 3. Copia o código-fonte restante
COPY ../../SharedKernel/ ../../SharedKernel/
COPY . .

# 4. Publish
RUN dotnet publish "AuthService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Stage 2 - Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "AuthService.dll"]