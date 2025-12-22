# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia TUDO
COPY . .

# Vai para o diretório do projeto e faz restore EXPLÍCITO
WORKDIR /app/backend/BillingService
RUN dotnet restore BillingService.csproj
RUN dotnet publish BillingService.csproj -c Release -o /out /p:UseAppHost=false

# Estágio 2: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "BillingService.dll"]