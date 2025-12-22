# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia TUDO da raiz
COPY . .

# RESTAURA usando caminho COMPLETO da raiz
RUN dotnet restore backend/BillingService/BillingService.csproj

# PUBLIC usando caminho COMPLETO
RUN dotnet publish backend/BillingService/BillingService.csproj -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BillingService.dll"]