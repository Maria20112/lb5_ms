FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["AuthService.csproj", "."]
COPY . .
RUN dotnet restore "./AuthService.csproj"

WORKDIR "/src"
RUN dotnet build "./AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/build

RUN dotnet publish "./AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]