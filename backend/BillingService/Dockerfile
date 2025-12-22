# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia TUDO
COPY . .

# --- COMANDOS DE DEBUG (REMOVER DEPOIS) ---
RUN echo "=========================================="
RUN echo "LISTANDO ARQUIVOS .CSPROJ ENCONTRADOS:"
RUN find . -name "*.csproj"
RUN echo "=========================================="
RUN echo "LISTANDO ESTRUTURA DA PASTA BACKEND:"
RUN ls -R backend || echo "Pasta backend não encontrada na raiz /src"
RUN echo "=========================================="
# ---------------------------------------------

# O comando original que está falhando
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