# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. PRIMEIRO copie APENAS os arquivos .csproj (caminhos COMPLETOS)
COPY ["SharedKernel/SharedKernel.csproj", "./"]
COPY ["backend/BillingService/BillingService.csproj", "./"]

# 2. Restaura as dependências
RUN dotnet restore "./BillingService.csproj"

# 3. Copie TODO o código fonte
COPY . .

# 4. Trabalha no diretório do projeto
WORKDIR "/src/backend/BillingService"
RUN dotnet publish "BillingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Final (Produção)
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Instala o 'curl', necessário para o healthcheck funcionar
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BillingService.dll"]