FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar arquivos de projeto
COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]
COPY ["backend/AuthService/AuthService.csproj", "backend/AuthService/"]

# Restaurar dependências
RUN dotnet restore "backend/AuthService/AuthService.csproj"

# Copiar todo o código
COPY . .

# Publicar
WORKDIR "/src/backend/AuthService"
RUN dotnet build "AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]