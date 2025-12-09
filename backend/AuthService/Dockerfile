# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copia os .csproj (com caminhos completos da raiz)
COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]
COPY ["backend/AuthService/AuthService.csproj", "backend/AuthService/"]

# 2. Restaura usando o caminho completo
RUN dotnet restore "backend/AuthService/AuthService.csproj"

# 3. Copia o código-fonte específico (mais rápido que "COPY . .")
COPY ["./SharedKernel/", "SharedKernel/"]
COPY ["./backend/AuthService/", "backend/AuthService/"]

# 4. Publica usando o caminho completo
WORKDIR "/src/backend/AuthService"
RUN dotnet publish "AuthService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Final (Produção)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Instala o 'curl', necessário para o healthcheck funcionar
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app

ENTRYPOINT ["dotnet", "AuthService.dll"]