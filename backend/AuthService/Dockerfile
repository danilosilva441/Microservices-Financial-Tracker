# Estágio 1: Build - Usa a imagem completa do SDK do .NET para compilar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copia o arquivo .csproj e restaura as dependências primeiro
COPY ["AuthService.csproj", "."]
RUN dotnet restore "./AuthService.csproj"

# Copia todo o resto do código e publica a aplicação
COPY . .
WORKDIR "/src/."
RUN dotnet build "AuthService.csproj" -c Release -o /app/build
RUN dotnet publish "AuthService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Final - Usa a imagem leve do ASP.NET Runtime para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]