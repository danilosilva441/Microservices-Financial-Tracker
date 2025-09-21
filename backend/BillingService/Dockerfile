# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo .csproj e restaura as dependências primeiro
COPY ["BillingService.csproj", "."]
RUN dotnet restore "./BillingService.csproj"

# Copia todo o resto do código e publica a aplicação
COPY . .
WORKDIR "/src/."
RUN dotnet publish "BillingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BillingService.dll"]