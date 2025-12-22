# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# --- AQUI ESTÁ O SEGREDO ---
# Copia TODO o contexto (backend, SharedKernel, etc) para dentro de /src
COPY . .

# Restaura usando o CAMINHO COMPLETO a partir da raiz
# Isso garante que ele ache o SharedKernel e o BillingService
RUN dotnet restore "backend/BillingService/BillingService.csproj"

# Publica usando o CAMINHO COMPLETO
RUN dotnet publish "backend/BillingService/BillingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Final (Produção)
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Instala curl para o healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BillingService.dll"]