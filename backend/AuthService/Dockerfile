# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# --- AQUI ESTÁ O SEGREDO ---
# Copia TODO o contexto
COPY . .

# Restaura usando o CAMINHO COMPLETO
RUN dotnet restore "backend/AuthService/AuthService.csproj"

# Publica usando o CAMINHO COMPLETO
RUN dotnet publish "backend/AuthService/AuthService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Final (Produção)
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Instala curl para o healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]