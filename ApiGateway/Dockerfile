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

WORKDIR "/src/ApiGateway"

RUN dotnet publish "./ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ApiGateway.dll"]