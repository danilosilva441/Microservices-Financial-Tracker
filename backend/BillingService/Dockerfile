# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copia os .csproj (com caminhos completos da raiz)
COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]
COPY ["backend/BillingService/BillingService.csproj", "backend/BillingService/"]

# 2. Restaura usando o caminho completo
RUN dotnet restore "backend/BillingService/BillingService.csproj"

# 3. Copia o código-fonte específico
COPY ["SharedKernel/", "SharedKernel/"]
COPY ["backend/BillingService/", "backend/BillingService/"]

# 4. Publica usando o caminho completo
WORKDIR "/src/backend/BillingService"
RUN dotnet publish "BillingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Final (Produção)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BillingService.dll"]