# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia primeiro o SharedKernel (que está em ../SharedKernel/)
COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]
# Copia o projeto AuthService
COPY ["backend/AuthService/AuthService.csproj", "backend/AuthService/"]

# Restaura as dependências
RUN dotnet restore "backend/AuthService/AuthService.csproj"

# Copia todo o código fonte
COPY SharedKernel/ ./SharedKernel/
COPY backend/AuthService/ ./backend/AuthService/

# Trabalha no diretório do projeto
WORKDIR "/src/backend/AuthService"
RUN dotnet publish "AuthService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Final (Produção)
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Instala o 'curl', necessário para o healthcheck funcionar
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]