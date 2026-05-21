FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY ["ApiGateway.sln", "./"]
COPY ["ApiGateway/ApiGateway.csproj", "ApiGateway/"]
COPY ["ApiGatewayTest/ApiGatewayTest.csproj", "ApiGatewayTest/"]

# Копируем весь код
COPY . .

RUN dotnet restore "ApiGateway.sln"
RUN dotnet build "ApiGateway.sln" -c $BUILD_CONFIGURATION

WORKDIR "/src/ApiGateway"

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .

WORKDIR /app/publish

# Копируем configuration.json из папки ApiGateway
COPY ApiGateway/configuration.json .

ENTRYPOINT ["dotnet", "ApiGateway.dll"]